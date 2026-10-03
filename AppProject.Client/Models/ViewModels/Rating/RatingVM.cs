using Newtonsoft.Json;
using System.Collections.Generic;


namespace AppProject.Client.Models.ViewModels.Rating;


public class RatingVM
{
    [JsonProperty("stars")]
    public int Stars { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("productId")]
    public int ProductId { get; set; }
}