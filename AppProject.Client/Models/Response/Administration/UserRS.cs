using Newtonsoft.Json;

namespace AppProject.Client.Models.Response.Administration;

public class UserRS
{
    [JsonProperty("userRSDTO")]
    public List<UserResponse>? UserResponses { get; set; }

    [JsonProperty("searchTerm")]
    public string? SearchTerm { get; set; }

    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; }

    [JsonProperty("pageSize")]
    public int PageSize { get; set; }

    [JsonProperty("totalCount")]
    public int TotalCount { get; set; }
    
    
    
    //=================================
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }
}

public class UserResponse
{
    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; }
}