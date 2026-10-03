using System.ComponentModel.DataAnnotations;
using AppProject.Client.Models.CustomValidators;
using Newtonsoft.Json;

namespace AppProject.Client.Models.ViewModels.Account;

public record RegisterVM
{
    [JsonProperty("email")]
    [Required]
    [EmailAddress]
    [EmailDomainValidator(AllowedDomain = "gmail.com",
        ErrorMessage = "Only gmail.com is allowed")]
    public string? Email { get; set; }

    [JsonProperty("userName")]
    [Required]
    public string? UserName { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [JsonProperty("password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password",
        ErrorMessage = "Password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }

    [JsonProperty("displayName")]
    public string? DisplayName { get; set; }
}