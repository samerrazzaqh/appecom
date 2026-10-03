using Core.DTO.AdministrationDTO;
using Core.DTO.AdministrationDTO.Roles;
using Core.DTO.AdministrationDTO.Users;

namespace Core.interfaces;

public interface IAdministration
{
    //============Roles==========================================================
    Task<IEnumerable<RolesDTO>> ListRoles();
    Task<RolesResponseDTO> CreateRole(RolesDTO rolesDto);
    Task<RolesResponseDTO> DeleteRole(string roleId);
    Task<RolesResponseDTO> GetUsersInRole(RolesDTO rolesDto);
    Task<RolesResponseDTO> EditRole(RolesEditDTO rolesEditDto);
    //============Users============================================================
    
    Task<UserDTO> ListUsers(UserDTO searchDto);
    Task<ClaimsRolesUser> GetUserClaimsAndRolesAsync(string userId);
    Task<ClaimsRolesUser> UpdateUserClaimsAndRolesAsync(UpdateClaimsRolesUserDTO dto);
    


}