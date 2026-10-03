using Newtonsoft.Json;
using System.Collections.Generic;

namespace AppProject.Client.Models.Response.Orders;

public class OrderRS
{
    [JsonProperty("id")] 
    public int Id { get; set; }

    [JsonProperty("buyerEmail")] 
    public string BuyerEmail { get; set; } = string.Empty;

    // تم التغيير إلى decimal لتستوعب القيم العشرية
    [JsonProperty("subTotal")] 
    public decimal SubTotal { get; set; }

    // تم التغيير إلى decimal
    [JsonProperty("total")] 
    public decimal Total { get; set; }

    // تم التغيير إلى DateTime لسهولة التعامل مع التواريخ
    [JsonProperty("orderDate")] 
    public DateTime OrderDate { get; set; }

    [JsonProperty("shippingAddress")] 
    public ShippingAddress ShippingAddress { get; set; } = new();

    [JsonProperty("orderItems")] 
    public List<OrderItems> OrderItems { get; set; } = new();

    [JsonProperty("deliveryMethod")] 
    public string DeliveryMethod { get; set; } = string.Empty;

    [JsonProperty("status")] 
    public string Status { get; set; } = string.Empty;
}

public class ShippingAddress
{
    [JsonProperty("id")] 
    public int Id { get; set; }

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

public class OrderItems
{
    [JsonProperty("productItemId")] 
    public int ProductItemId { get; set; }

    [JsonProperty("mainImage")] 
    public string MainImage { get; set; } = string.Empty;

    [JsonProperty("productName")] 
    public string ProductName { get; set; } = string.Empty;

    // تم التغيير إلى decimal
    [JsonProperty("price")] 
    public decimal Price { get; set; }

    [JsonProperty("quntity")] 
    public int Quntity { get; set; }
}
