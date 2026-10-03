using Newtonsoft.Json;

namespace AppProject.Client.Models.ViewModels.Orders;

// --- Request Models ---
public class OrderVM
{
    [JsonProperty("deliveryMethodId")]
    public int DeliveryMethodId { get; set; }

    [JsonProperty("basketId")]
    public string BasketId { get; set; } = string.Empty;

    [JsonProperty("shipAddress")]
    public ShipAddress ShipAddress { get; set; } = new();
}

public class ShipAddress
{
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("zipCode")]
    public string ZipCode { get; set; } = string.Empty;

    [JsonProperty("street")]
    public string Street { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;
}