﻿using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdministratorServices(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IdentityResult> CreateRole([FromBody] RoleModel model)
        {
            var role = new IdentityRole { Name = model.Name};
            var result =  _roleManager.CreateAsync(role);
            return result;
        }

        public Dictionary<string,string> GetUserClaims()
        {
            var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (identity.Claims.Count() == 0) return null ;

            var values = new Dictionary<string,string>
            {
                { "username" , identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value },
                { "Email" , identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value },
                { "Roles", identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value },
                { "Permission", identity.FindFirst("IsAllowed").Value },
            };

            return values;
        }
    }
}
