using Newtonsoft.Json;

namespace AppProject.Client.Models.Response.Product;

public class GetProductRS
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal NewPrice { get; set; }
    public decimal OldPrice { get; set; }
    
    public List<PhotoVM> Photos { get; set; } = new();
    
    [JsonProperty("categoryName")]
    public string CategoryName { get; set; } = string.Empty;

    public double Rating { get; set; }
}

public class PhotoVM
{
    public string ImageName { get; set; } = string.Empty;
    public int ProductId { get; set; }
}