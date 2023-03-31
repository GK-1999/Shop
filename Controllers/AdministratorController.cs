using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Administration;
using Shop.Services.Interfaces;
using System.Security.Claims;

namespace Shop.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAdministratorServices _administrator;
        private readonly UserManager<IdentityUser> _userManager;
        private dynamic data;
        public AdministratorController(RoleManager<IdentityRole> roleManager,
                                       IAdministratorServices administrator,
                                       UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _administrator = administrator;
            _userManager = userManager;

            data = _administrator.GetUserClaims();
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model.Name == null) return BadRequest("Role Not Specified");

            if (data["Name"] == null) return BadRequest("No User Logged In");
            if (data["Roles"] != "SuperAdmin") return BadRequest("Permission Denied");

            if (await _roleManager.RoleExistsAsync(model.Name)) return BadRequest("Role Already Exists");

            var result = await _administrator.CreateRole(model);
            if (result.Succeeded) return Ok($"{model.Name} Role Created Sucessfully");

            return BadRequest(result.Errors);
        }

        [HttpGet("GetRoles")]
        public dynamic ViewRoles()
        {
            if (data["Name"].ToString() == null) return BadRequest("No User Logged In");
            if (data["Roles"] != "SuperAdmin") return BadRequest("Permission Denied");

            return _roleManager.Roles.ToList();
        }

        [HttpGet("GetUsersWithRole")]
        public async Task<dynamic> ListRoleUsers(string name)
        {
            if (data["Name"] == "") return BadRequest("No Role Specified");
            var userRole = data["Roles"];

            if(userRole == null) return BadRequest("User Not Logged In");
            
            if (userRole == "Admin")
                    if (name == "SuperAdmin" || name == "Admin") 
                        return BadRequest("You are Not Allowed to Access this");
            
            var Users = await _userManager.GetUsersInRoleAsync(name);

            return Ok(Users);
        }
    }
}
