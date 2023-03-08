using Microsoft.AspNetCore.Identity;
using Shop.Services.Interfaces;
using System.Data;
using System.Xml.Linq;

namespace Shop.Services.Implementations
{
    public class AdministratorServices : IAdministratorServices
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        public AdministratorServices(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public Task<IdentityResult> AddUserToRole(IdentityUser user, string name)
        {

            return _userManager.AddToRoleAsync(user, name);
        }

        public Task<IdentityResult> CreateRole(string name)
        {
                var role = new IdentityRole { Name = name };
                return _roleManager.CreateAsync(role);
        }
    }
}
