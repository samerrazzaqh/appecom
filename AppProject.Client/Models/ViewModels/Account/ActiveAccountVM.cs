namespace AppProject.Client.Models.ViewModels.Account;

public record ActiveAccountVM
{
    public string Email { get; set; }
    public string Token { get; set; }
}