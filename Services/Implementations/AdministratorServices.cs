using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Administration;
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

        public Task<IdentityResult> CreateRole([FromBody] RoleModel model)
        {
            var role = new IdentityRole { Name = model.Name};
            var result =  _roleManager.CreateAsync(role);
            return result;
        }
    }
}
