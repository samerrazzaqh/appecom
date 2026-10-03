using AppProject.Client.Models.Response.Profile;
using AppProject.Client.Models.ViewModels.Profile;

namespace AppProject.Client.Service.Profile;

public interface IProfileService
{
    Task<AddressRS?> getUserInfoAddress();
    Task<bool> UpdateAddress(ShipAddressVM addressVM);
}