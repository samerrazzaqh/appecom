namespace Core.DTO.AdministrationDTO.Roles;

public class RolesResponseDTO
{
    
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    
    public List<UserRoleRS> UserRoleRS { get; set; }
    public int TotalCount { get; set; }
    
    public string message { get; set; }
    public bool success { get; set; }
}


public class UserRoleRS
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public bool IsSelected { get; set; }
}
