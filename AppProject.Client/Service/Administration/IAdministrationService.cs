using AppProject.Client.Models.Response.Administration;
using AppProject.Client.Models.ViewModels.Administration;

namespace AppProject.Client.Service.Administration;

public interface IAdministrationService
{
    //============Roles==========================================================
    Task<IEnumerable<RolesRS>> ListRoles();
    Task<RolesResponse> CreateRole(RolesVM rolesVm);
    Task<RolesResponse> DeleteRole(string roleId);
    Task<RolesResponse<DataRoles>> GetUsersInRole(RolesVM rolesVm);
    Task<RolesResponse> EditRole(RolesEditVM rolesEditDto);
    //============Users============================================================
    
    Task<UserRS> ListUsers(UserVM userVm);
    Task<ClaimsRoelsUserRS> GetEditeUserClaims(string userId);
    Task<ClaimsRoelsUserRS> EditeUserClaims(ClaimsRolesUserVM claimsRolesUserVM);
    
}