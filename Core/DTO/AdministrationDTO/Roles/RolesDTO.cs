namespace Core.DTO.AdministrationDTO.Roles;

public class RolesDTO
{
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}

public class RolesEditDTO
{
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public List<UserRole>? UserRole { get; set; }
}

public class UserRole
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public bool IsSelected { get; set; }
}