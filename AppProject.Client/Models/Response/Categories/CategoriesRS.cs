using Newtonsoft.Json;

namespace AppProject.Client.Models.Response.Categories;


// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
public class CategoriesRS
{
    public int id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    
    
    // ==============================================
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}