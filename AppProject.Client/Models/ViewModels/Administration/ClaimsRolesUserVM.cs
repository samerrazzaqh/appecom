using Newtonsoft.Json;
using System.Collections.Generic;

namespace AppProject.Client.Models.ViewModels.Administration;

public class ClaimsRolesUserVM
{
    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("block")]
    public bool Block { get; set; }

    [JsonProperty("claims")]
    public List<Claims> Claims { get; set; }

    [JsonProperty("roles")]
    public List<Roles> Roles { get; set; }
}

public class Claims
{
    [JsonProperty("claimType")]
    public string ClaimType { get; set; }

    [JsonProperty("isSelected")]
    public bool IsSelected { get; set; }
}

public class Roles
{
    [JsonProperty("roleId")]
    public string RoleId { get; set; }

    [JsonProperty("roleName")]
    public string RoleName { get; set; }

    [JsonProperty("isSelected")]
    public bool IsSelected { get; set; }
}