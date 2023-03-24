using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Administration;
using Shop.Services.Interfaces;
using System.Security.Claims;

namespace Shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAdministratorServices _administrator;
        private readonly UserManager<IdentityUser> _userManager;
        public AdministratorController(RoleManager<IdentityRole> roleManager,
                                       IAdministratorServices administrator,
                                       UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _administrator = administrator;
            _userManager = userManager;
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model.Name == null) return BadRequest("Role Not Specified");

            dynamic data = _administrator.GetUserClaims();

            if (data == null) return BadRequest("No User Logged In");
            if (data.Data.userRole != "SuperAdmin") return BadRequest("Permission Denied");

            if (await _roleManager.RoleExistsAsync(model.Name)) return BadRequest("Role Already Exists");

            var result = await _administrator.CreateRole(model);
            if (result.Succeeded) return Ok($"{model.Name} Role Created Sucessfully");

            return BadRequest(result.Errors);
        }

        [HttpGet("GetRoles")]
        public dynamic ViewRoles()
        {
            dynamic data = _administrator.GetUserClaims();

            if (data == null) return BadRequest("No User Logged In");
            if (data.Data.userRole != "SuperAdmin") return BadRequest("Permission Denied");

            return _roleManager.Roles.ToList();
        }

        [HttpPost("GetUsersWithRole")]
        public async Task<dynamic> ListRoleUsers(string name)
        {
            dynamic userData = _administrator.GetUserClaims();
            if (userData == null) return BadRequest(userData.Data.Message);

            if (name == null) return BadRequest("No Role Specified");
            var userRole = userData.Data.userRole;

            if(userRole == null) return BadRequest("User Not Logged In");
            
            if (userRole == "Admin")
                    if (name == "SuperAdmin" || name == "Admin") 
                        return BadRequest("You are Not Allowed to Access this");
            
            var Users = await _userManager.GetUsersInRoleAsync(name);

            return Ok(Users);
        }
    }
}
