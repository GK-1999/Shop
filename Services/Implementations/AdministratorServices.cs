using Microsoft.AspNetCore.Identity;
using Shop.Services.Interfaces;

namespace Shop.Services.Implementations
{
    public class AdministratorServices : IAdministratorServices
    {
        public Task<IdentityResult> AddUserToRole(IdentityUser user, string role)
        {
            throw new NotImplementedException();
        }

        Task<IdentityResult> IAdministratorServices.CreateRole(string name)
        {
            throw new NotImplementedException();
        }
    }
}
