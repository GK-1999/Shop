using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.IdentityModel.Tokens;
using Shop.Models.Administration;
using Shop.Models.Response;
using Shop.Services.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Xml.Linq;

namespace Shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAdministratorServices _administrator;

        public AdministratorController(RoleManager<IdentityRole> roleManager,
                                       UserManager<IdentityUser> userManager,
                                       IAdministratorServices administrator)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _administrator = administrator;
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model.Name == null) return BadRequest("Role Not Specified");

            if (await _roleManager.RoleExistsAsync(model.Name)) return BadRequest("Role Already Exists");

            var result = await _administrator.CreateRole(model);
            if (result.Succeeded) return Ok("Role Created Sucessfully");

            return BadRequest("Role Not Created");
        }

        [HttpGet("GetRoles")]
        public dynamic ViewRoles()
        {
            return _roleManager.Roles.ToList();
        }

        [HttpPost("GetUsersWithRole")]
        public async Task<dynamic> ListRoles(string name)
        {

            //var user = await _userManager.GetUserAsync(User);
            //if (user == null) return BadRequest();
            //if (userRole == "SuperAdmin") return await _userManager.GetUsersInRoleAsync(name);
            //if (userRole == "Admin")
            //if (name == "SuperAdmin" || name == "Admin") return BadRequest("You are Not Allowed to Access this");

            return BadRequest("You are Not Allowed to Access this");
        }
    }
}
