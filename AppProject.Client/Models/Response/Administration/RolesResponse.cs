using Newtonsoft.Json;

namespace AppProject.Client.Models.Response.Administration;


public class RolesResponse
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }
}

public class RolesResponse<T>
{
    [JsonProperty("data")]
    public T? Data { get; set; }

    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }
}




public class UserRoleRS
{
    [JsonProperty("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonProperty("isSelected")]
    public bool IsSelected { get; set; }
}



public class DataRoles
{
    [JsonProperty("roleId")]
    public string RoleId { get; set; } = string.Empty;

    [JsonProperty("roleName")]
    public string RoleName { get; set; } = string.Empty;

    [JsonProperty("userRoleRS")]
    public List<UserRoleRS> UserRoleRS { get; set; } = new();
    
    [JsonProperty("totalCount")]
    public int TotalCount { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("success")]
    public bool Success { get; set; }
}