using Newtonsoft.Json;
using System.Collections.Generic;

namespace AppProject.Client.Models.Response.Product;

// ProductRS myDeserializedClass = JsonConvert.DeserializeObject<ProductRS>(myJsonResponse);

public class ProductRS<T>
{
    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; }

    [JsonProperty("pageSize")]
    public int PageSize { get; set; }

    [JsonProperty("totalCount")]
    public int TotalCount { get; set; }

    [JsonProperty("data")]
    public List<DataProduct> Data { get; set; }
}

public class Photos
{
    [JsonProperty("imageName")]
    public string ImageName { get; set; }

    [JsonProperty("productId")]
    public int ProductId { get; set; }
}

public class DataProduct
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("newPrice")]
    public decimal NewPrice { get; set; }

    [JsonProperty("oldPrice")]
    public decimal OldPrice { get; set; }

    [JsonProperty("photos")]
    public virtual List<Photos> Photos { get; set; }

    [JsonProperty("categoryName")]
    public string CategoryName { get; set; }

    [JsonProperty("rating")]
    public double Rating { get; set; }
}






//==================================================
// public class PaginationResponse<T>
// {
//     public int PageNumber { get; set; }
//     public int PageSize { get; set; }
//     public int TotalCount { get; set; }
//     public List<T> Data { get; set; } = new();
// }