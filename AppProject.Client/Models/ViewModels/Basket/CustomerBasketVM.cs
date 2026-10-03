using Newtonsoft.Json;
using System.Collections.Generic;

namespace AppProject.Client.Models.ViewModels.Basket;


public class CustomerBasketVM
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("paymentIntentId")]
    public string PaymentIntentId { get; set; }

    [JsonProperty("clientSecret")]
    public string ClientSecret { get; set; }

    [JsonProperty("basketItems")]
    public List<BasketItems> BasketItems { get; set; } = new List<BasketItems>();
}


public class BasketItems
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("image")]
    public string Image { get; set; }

    // تصحيح السعر إلى decimal والـ Quantity إلى الكلمة الصحيحة
    [JsonProperty("qunatity")]
    public int Qunatity { get; set; }

    [JsonProperty("price")]
    public decimal Price { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }
}