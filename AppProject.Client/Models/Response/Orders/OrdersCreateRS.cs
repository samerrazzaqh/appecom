using Newtonsoft.Json;
using System.Collections.Generic;

namespace AppProject.Client.Models.Response.Orders;

// --- Response Models ---
public class OrdersCreateRS
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("buyerEmail")]
    public string BuyerEmail { get; set; } = string.Empty;

    [JsonProperty("subTotal")]
    public decimal SubTotal { get; set; }

    [JsonProperty("orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonProperty("paymentIntentId")]
    public string PaymentIntentId { get; set; } = string.Empty;

    [JsonProperty("shippingAddress")]
    public ShippingAddressRS ShippingAddress { get; set; } = new();

    [JsonProperty("orderItems")]
    public List<OrderItemsRS> OrderItems { get; set; } = new();

    [JsonProperty("deliveryMethod")]
    public DeliveryMethodRS DeliveryMethod { get; set; } = new();

    [JsonProperty("status")]
    public int Status { get; set; }
}

public class ShippingAddressRS
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

public class OrderItemsRS
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("productItemId")]
    public int ProductItemId { get; set; }

    [JsonProperty("mainImage")]
    public string MainImage { get; set; } = string.Empty;

    [JsonProperty("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonProperty("price")]
    public decimal Price { get; set; }

    [JsonProperty("quntity")]
    public int Quntity { get; set; }
}

public class DeliveryMethodRS
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