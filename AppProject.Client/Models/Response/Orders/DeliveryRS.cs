using Newtonsoft.Json;
using System.Collections.Generic;

namespace AppProject.Client.Models.Response.Orders;

public class DeliveryRS
{
    [JsonProperty("id")] 
    public int Id { get; set; }

    [JsonProperty("name")] 
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("price")] 
    public decimal Price { get; set; }

    [JsonProperty("deliveryTime")] 
    public string DeliveryTime { get; set; } = string.Empty;

    [JsonProperty("description")] 
    public string Description { get; set; } = string.Empty;
}