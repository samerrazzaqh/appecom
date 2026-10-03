using System.ComponentModel.DataAnnotations;

namespace AppProject.Client.Models.ViewModels.Account;

public record LoginVM
{
    
    [Required] public string? Email { get; set; }


    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}