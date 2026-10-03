using System.Text.Json.Serialization;

namespace Core.DTO.AdministrationDTO.Users;

public class ClaimsRolesUser
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool Block { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ClaimsUser>? Claims { get; set; } = new();
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<RolesUser>? Roles { get; set; } = new();
    
    public string message { get; set; }
    public bool success { get; set; }
}


public class UpdateClaimsRolesUserDTO
{
    public string UserId { get; set; } = string.Empty;
    public bool Block { get; set; }
    public List<RolesUser> Roles { get; set; } = new();
    public List<ClaimsUser> Claims { get; set; } = new();
}


public class RolesUser
{
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool IsSelected { get; set; }
}

public class ClaimsUser
{
    public string? ClaimId { get; set; }
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
    public bool IsSelected { get; set; }
}