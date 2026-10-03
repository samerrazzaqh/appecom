using Newtonsoft.Json;

namespace AppProject.Client.Models.ViewModels.Administration;

public class UserVM
{
    [JsonProperty("searchTerm")] public string? SearchTerm { get; set; }

    [JsonProperty("pageNumber")] public int PageNumber { get; set; } = 1;

    [JsonProperty("pageSize")] public int PageSize { get; set; } = 5;
}