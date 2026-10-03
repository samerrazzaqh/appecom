using Newtonsoft.Json;
using System.Collections.Generic;

namespace AppProject.Client.Models.Response.Rating;

public class RatingRS
{
    [JsonProperty("stars")]
    public int Stars { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; }

    [JsonProperty("reviewTime")]
    public DateTime ReviewTime { get; set; }
}