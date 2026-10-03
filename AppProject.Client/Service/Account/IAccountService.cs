using AppProject.Client.Models.Response.Account;
using AppProject.Client.Models.ViewModels.Account;

namespace AppProject.Client.Service.Account;

public interface IAccountService
{
    Task<LoginRS> LoginAsync(LoginVM loginVM);
    
    Task<RegisterRS> RegisterAsync(RegisterVM registerVm);
    Task<bool> Active(ActiveAccountVM activeAccountVm );
    
    Task<bool> ForgotPasswordbyEmail(string Email);
    Task<ResetPasswordbyEmailRS> ResetPassword(RestPasswordVM restPasswordVm);
}