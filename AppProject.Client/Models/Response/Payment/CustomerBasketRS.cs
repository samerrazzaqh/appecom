using System.Text.Json.Serialization;
using Newtonsoft.Json;
namespace AppProject.Client.Models.Response.Payment;

public class CustomerBasketRS
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("paymentIntentId")]
    public string PaymentIntentId { get; set; } = string.Empty;

    [JsonPropertyName("clientSecret")]
    public string ClientSecret { get; set; } = string.Empty;

    [JsonPropertyName("basketItems")]
    public List<BasketItems> BasketItems { get; set; } = new();
}

public class BasketItems
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;

    [JsonPropertyName("qunatity")]
    public int Qunatity { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; } // تم تغييرها من int إلى decimal

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
}