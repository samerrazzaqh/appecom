using AppProject.Client.Models.Authentication;
using Newtonsoft.Json;

namespace AppProject.Client.Models.Response.Account;

public class LoginRS
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}