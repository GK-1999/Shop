using Microsoft.AspNetCore.Identity;

namespace Shop.Services.Interfaces
{
    public interface IAdministratorServices
    {
        Task<IdentityResult> CreateRole(string name);
        Task<IdentityResult> AddUserToRole(IdentityUser user, string role);

    }
}
