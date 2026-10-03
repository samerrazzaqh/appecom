using System.Security.Claims;
using Core.DTO.AdministrationDTO;
using Core.DTO.AdministrationDTO.Roles;
using Core.DTO.AdministrationDTO.Users;
using Core.Entities;
using Core.interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositries;

public class AdministrationRepositry : IAdministration
{
    private readonly UserManager<AppUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly AppDbContext context;

    public AdministrationRepositry(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
        AppDbContext context)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.context = context;
    }

//==========================ListRoles===============================================================================

    public async Task<IEnumerable<RolesDTO>> ListRoles()
    {
        var roles = await roleManager.Roles
            .Select(r => new RolesDTO
            {
                RoleId = r.Id,
                RoleName = r.Name,
            })
            .ToListAsync();

        return roles;
    }
//==========================CreateRole===============================================================================

    public async Task<RolesResponseDTO> CreateRole(RolesDTO? rolesDto)
    {
        if (rolesDto == null || string.IsNullOrWhiteSpace(rolesDto.RoleName))
        {
            return new RolesResponseDTO { message = "Required field", success = false };
        }

        IdentityRole identityRole = new IdentityRole { Name = rolesDto.RoleName.Trim() };

        IdentityResult result = await roleManager.CreateAsync(identityRole);

        if (result.Succeeded)
        {
            return new RolesResponseDTO { message = "Create Role", success = true };
        }

        string errors = string.Join(", ", result.Errors.Select(e => e.Description));

        return new RolesResponseDTO { message = $"Error CreateRole: {errors}", success = false };
    }

//==========================DeleteRole===============================================================================

    public async Task<RolesResponseDTO> DeleteRole(string roleId)
    {
        if (string.IsNullOrWhiteSpace(roleId))
        {
            return new RolesResponseDTO { message = "Role ID is required", success = false };
        }

        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return new RolesResponseDTO { message = "Role with Id cannot be found", success = false };
        }

        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any())
        {
            return new RolesResponseDTO
            {
                message =
                    $"Cannot delete '{role.Name}' because users are assigned to it. Please remove users from the role first.",
                success = false
            };
        }

        try
        {
            var result = await roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return new RolesResponseDTO { message = "Role deleted successfully", success = true };
            }

            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new RolesResponseDTO { message = $"Error DeleteRole: {errors}", success = false };
        }
        catch (DbUpdateException)
        {
            return new RolesResponseDTO
            {
                message = $"Error DeleteRole: {role.Name} role is in use and cannot be deleted.",
                success = false
            };
        }
    }
    //==========================GetUsersInRole===============================================================================

    public async Task<RolesResponseDTO> GetUsersInRole(RolesDTO rolesDto)
    {
        if (string.IsNullOrWhiteSpace(rolesDto.RoleId))
        {
            return new RolesResponseDTO { message = "Role ID is required", success = false };
        }

        var role = await roleManager.FindByIdAsync(rolesDto.RoleId);
        if (role == null)
        {
            return new RolesResponseDTO { message = "Role with Id cannot be found", success = false };
        }

        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
        var userIdsInRole = new HashSet<string>(usersInRole.Select(u => u.Id));

        var query = userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(rolesDto.SearchTerm))
        {
            string search = rolesDto.SearchTerm.Trim().ToLower();
            query = query.Where(u => u.UserName!.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync();
        var pagedUsers = await query
            .OrderBy(u => u.UserName) // الترتيب ضروري قبل Skip و Take
            .Skip((rolesDto.PageNumber - 1) * rolesDto.PageSize)
            .Take(rolesDto.PageSize)
            .Select(user => new UserRoleRS
            {
                UserId = user.Id,
                UserName = user.UserName,
                IsSelected = userIdsInRole.Contains(user.Id)
            })
            .ToListAsync();

        return new RolesResponseDTO
        {
            RoleId = role.Id,
            RoleName = role.Name,
            success = true,
            message = "Users retrieved successfully",
            UserRoleRS = pagedUsers,
            TotalCount = totalCount
        };
    }

    //==========================EditRole===============================================================================
    public async Task<RolesResponseDTO> EditRole(RolesEditDTO rolesEditDto)
    {
        // 1. التحقق من المدخلات الأساسية
        if (rolesEditDto == null ||
            string.IsNullOrWhiteSpace(rolesEditDto.RoleId) ||
            string.IsNullOrWhiteSpace(rolesEditDto.RoleName))
        {
            return new RolesResponseDTO
                { message = "Required field (RoleId and RoleName are mandatory)", success = false };
        }

        // 2. البحث عن الدور
        var role = await roleManager.FindByIdAsync(rolesEditDto.RoleId);
        if (role == null)
        {
            return new RolesResponseDTO { message = "Role with Id cannot be found", success = false };
        }

        // 3. تحديث اسم الدور
        role.Name = rolesEditDto.RoleName.Trim();
        var result = await roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            // 4. معالجة قائمة المستخدمين المرسلة من الصفحة الحالية
            if (rolesEditDto.UserRole != null && rolesEditDto.UserRole.Any())
            {
                foreach (var userRole in rolesEditDto.UserRole)
                {
                    // البحث عن المستخدم (سواء بواسطة Id أو UserName)
                    var user = !string.IsNullOrWhiteSpace(userRole.UserId)
                        ? await userManager.FindByIdAsync(userRole.UserId)
                        : await userManager.FindByNameAsync(userRole.UserName!);

                    if (user == null) continue;

                    var isInRole = await userManager.IsInRoleAsync(user, role.Name!);

                    // إضافة الدور للمستخدم إذا تم تحديده ولم يكن يملكه
                    if (userRole.IsSelected && !isInRole)
                    {
                        await userManager.AddToRoleAsync(user, role.Name!);
                        await userManager.UpdateSecurityStampAsync(user);
                    }
                    // إزالة الدور من المستخدم إذا تم إلغاء تحديده وكان يملكه
                    else if (!userRole.IsSelected && isInRole)
                    {
                        await userManager.RemoveFromRoleAsync(user, role.Name!);
                        await userManager.UpdateSecurityStampAsync(user);
                    }
                }
            }

            return new RolesResponseDTO { message = "Role updated and users synced successfully", success = true };
        }

        string errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return new RolesResponseDTO { message = $"Error EditRole: {errors}", success = false };
    }

//==========================ListRoles===============================================================================

    public async Task<UserDTO> ListUsers(UserDTO searchDto)
    {
        var query = userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
        {
            query = query.Where(u => u.UserName != null && u.UserName.Contains(searchDto.SearchTerm));
        }

        var totalCount = await query.CountAsync();

        var userList = await query
            .OrderBy(u => u.UserName)
            .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .Select(r => new UserResponseDTO
            {
                UserId = r.Id,
                UserName = r.UserName
            })
            .ToListAsync();

        return new UserDTO
        {
            TotalCount = totalCount,
            PageNumber = searchDto.PageNumber,
            PageSize = searchDto.PageSize,
            SearchTerm = searchDto.SearchTerm,
            UserRSDTO = userList
        };
    }


    //==========================GetUserClaimsAndRolesAsync=======================================================
    public async Task<ClaimsRolesUser> GetUserClaimsAndRolesAsync(string userId)
    {
        try
        {
            // 1. البحث عن المستخدم
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ClaimsRolesUser
                {
                    success = false,
                    message = "User not found."
                };
            }

            // 2. جلب الأدوار
            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = await roleManager.Roles.ToListAsync();

            // 3. جلب الـ Claims مباشرة من DbContext بقيم الـ Id الحقيقية
            var userClaimsDb = await context.UserClaims
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // 4. إعداد كائن النتيجة
            return new ClaimsRolesUser
            {
                success = true,
                message = "Data retrieved successfully.",
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Block = user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,

                // ربط الأدوار
                Roles = allRoles.Select(r => new RolesUser
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    IsSelected = userRoles.Contains(r.Name!)
                }).ToList(),

                // قراءة الـ Id الحقيقي من جدول قاعدة البيانات
                Claims = userClaimsDb.Select(c => new ClaimsUser
                {
                    ClaimId = c.Id.ToString(), // سيجلب 1001, 1002, 1003... إلخ
                    ClaimType = c.ClaimType,
                    ClaimValue = c.ClaimValue,
                    IsSelected = c.ClaimValue != null && c.ClaimValue.Equals("true", StringComparison.OrdinalIgnoreCase)
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            return new ClaimsRolesUser
            {
                success = false,
                message = $"Error: {ex.Message}"
            };
        }
    }

    //==========================UpdateUserClaimsAndRolesAsync=======================================================

    public async Task<ClaimsRolesUser> UpdateUserClaimsAndRolesAsync(UpdateClaimsRolesUserDTO dto)
{
    try
    {
        // 1. البحث عن المستخدم بواسطة dto.UserId
        var user = await userManager.FindByIdAsync(dto.UserId);
        if (user == null)
        {
            return new ClaimsRolesUser { success = false, message = "User not found." };
        }

        // 2. تحديث الحظر (Block/Lockout)
        if (dto.Block)
            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        else
            await userManager.SetLockoutEndDateAsync(user, null);

        // 3. تحديث الأدوار (Roles)
        var currentRoles = await userManager.GetRolesAsync(user);
        foreach (var role in dto.Roles)
        {
            if (role.IsSelected && !currentRoles.Contains(role.RoleName!))
                await userManager.AddToRoleAsync(user, role.RoleName!);
            else if (!role.IsSelected && currentRoles.Contains(role.RoleName!))
                await userManager.RemoveFromRoleAsync(user, role.RoleName!);
        }

        // 4. تحديث الـ Claims بدون حذف (تعديل قيمة التفعيل داخل ClaimValue)
        var userClaimsDb = await context.UserClaims
            .Where(c => c.UserId == dto.UserId)
            .ToListAsync();

        foreach (var claimDto in dto.Claims)
        {
            var existingClaim = userClaimsDb.FirstOrDefault(c => c.ClaimType == claimDto.ClaimType);
            
            // هنا نعتمد على isSelected لتحكم القيمة التي تخزن في قاعدة البيانات (true / false)
            string targetValue = claimDto.IsSelected ? "true" : "false";

            if (existingClaim != null)
            {
                // إذا كان الـ Claim موجوداً مسبقاً، نكتفي بتحديث قيمته دون حذفه (سواء أصبح true أو false)
                if (existingClaim.ClaimValue != targetValue)
                {
                    existingClaim.ClaimValue = targetValue;
                    context.UserClaims.Update(existingClaim);
                }
            }
            else
            {
                // إذا لم يكن السطر موجوداً أصلاً، نقوم بإضافته
                context.UserClaims.Add(new IdentityUserClaim<string>
                {
                    UserId = dto.UserId,
                    ClaimType = claimDto.ClaimType,
                    ClaimValue = targetValue
                });
            }
        }
        
        await userManager.UpdateSecurityStampAsync(user);

        // حفظ التعديلات على الـ Claims بنفس الـ Id بدون حذف أي عنصر
        await context.SaveChangesAsync();

        // 5. إعادة جلب البيانات المحدثة للإرجاع
        var updatedRoles = await userManager.GetRolesAsync(user);
        var allRoles = await roleManager.Roles.ToListAsync();
        
        var updatedClaimsDb = await context.UserClaims
            .Where(c => c.UserId == dto.UserId)
            .ToListAsync();

        return new ClaimsRolesUser
        {
            success = true,
            message = "User claims and roles updated successfully.",
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Block = user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
            Roles = allRoles.Select(r => new RolesUser
            {
                RoleId = r.Id,
                RoleName = r.Name,
                IsSelected = updatedRoles.Contains(r.Name!)
            }).ToList(),
            
            // إرجاع الـ Claims مع بقاء السطور ثابته في الداتا بيز وقراءة حالة isSelected المحدثة
            Claims = updatedClaimsDb.Select(c => new ClaimsUser
            {
                ClaimId = c.Id.ToString(),
                ClaimType = c.ClaimType,
                ClaimValue = c.ClaimValue,
                IsSelected = c.ClaimValue != null && c.ClaimValue.Equals("true", StringComparison.OrdinalIgnoreCase)
            }).ToList()
        };
    }
    catch (Exception ex)
    {
        return new ClaimsRolesUser
        {
            success = false,
            message = $"Error updating user: {ex.Message}"
        };
    }
}
    
    
    
    
}