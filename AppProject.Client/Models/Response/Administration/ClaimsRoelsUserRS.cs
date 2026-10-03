using Newtonsoft.Json;
namespace AppProject.Client.Models.Response.Administration;


public class ClaimsRoelsUserRS
{
    [JsonProperty("data")]
    public DataClaimsRoelsUser DataClaimsRoelsUser { get; set; }

    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}

public class ClaimsUser
{
    [JsonProperty("claimId")]
    public string ClaimId { get; set; }

    [JsonProperty("claimType")]
    public string ClaimType { get; set; }

    [JsonProperty("claimValue")]
    public string ClaimValue { get; set; }

    [JsonProperty("isSelected")]
    public bool IsSelected { get; set; }
}

public class RolesUser
{
    [JsonProperty("roleId")]
    public string RoleId { get; set; }

    [JsonProperty("roleName")]
    public string RoleName { get; set; }

    [JsonProperty("isSelected")]
    public bool IsSelected { get; set; }
}

public class DataClaimsRoelsUser
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("block")]
    public bool Block { get; set; }

    [JsonProperty("claims")]
    public List<ClaimsUser> ClaimsUser { get; set; }

    [JsonProperty("roles")]
    public List<RolesUser> RolesUser { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("success")]
    public bool Success { get; set; }
}