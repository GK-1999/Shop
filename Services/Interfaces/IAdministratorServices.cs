using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Administration;

namespace Shop.Services.Interfaces
{
    public interface IAdministratorServices
    {
        Task<IdentityResult> CreateRole(RoleModel name);
        Task<IdentityResult> AddUserToRole(IdentityUser user, string role);

    }
}
