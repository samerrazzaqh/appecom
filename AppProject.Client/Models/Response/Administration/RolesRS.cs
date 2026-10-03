using Newtonsoft.Json;

namespace AppProject.Client.Models.Response.Administration;

public class RolesRS
{
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    
    
    // ==============================================
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}