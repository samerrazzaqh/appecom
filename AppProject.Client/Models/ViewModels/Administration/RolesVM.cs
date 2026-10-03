using AppProject.Client.Models.Response.Administration;
using Newtonsoft.Json;

namespace AppProject.Client.Models.ViewModels.Administration;
public class RolesVM
{
    [JsonProperty("roleId")]
    public string RoleId { get; set; }

    [JsonProperty("roleName")]
    public string RoleName { get; set; }

    [JsonProperty("searchTerm")]
    public string SearchTerm { get; set; }

    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; }

    [JsonProperty("pageSize")]
    public int PageSize { get; set; }
}


public class RolesEditVM
{
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public List<UserRoleRS>? UserRole { get; set; }
}

public class UserRole
{
    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonProperty("isSelected")]
    public bool IsSelected { get; set; }
}