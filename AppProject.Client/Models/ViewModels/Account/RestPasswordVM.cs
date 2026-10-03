using System.ComponentModel.DataAnnotations;

namespace AppProject.Client.Models.ViewModels.Account;

public record RestPasswordVM
{
    public string? Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password",
        ErrorMessage = "Password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }
    
    public string? Token { get; set; }
}