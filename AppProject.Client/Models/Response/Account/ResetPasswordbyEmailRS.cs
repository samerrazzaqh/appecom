using Newtonsoft.Json;

namespace AppProject.Client.Models.Response.Account;

public class ResetPasswordbyEmailRS
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}