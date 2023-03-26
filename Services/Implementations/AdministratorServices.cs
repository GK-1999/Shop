using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Administration;
using Shop.Models.Response;
using Shop.Services.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Xml.Linq;

namespace Shop.Services.Implementations
{
    public class AdministratorServices : IAdministratorServices
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdministratorServices(RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IdentityResult> CreateRole([FromBody] RoleModel model)
        {
            var role = new IdentityRole { Name = model.Name};
            var result =  _roleManager.CreateAsync(role);
            return result;
        }

        public response GetUserClaims()
        {
            var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (identity.Claims.Count() == 0) return new response { Message ="User Not LoggedIn" , StatusCode = 400};

            string userRole = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
            string userName = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            string userEmail = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            return new response {Message = "Claims Retrived Sucessfully" ,StatusCode = 200 , Data = new { userEmail, userName, userRole } };
        }
    }
}
