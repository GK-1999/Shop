using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Administration;
using Shop.Services.Interfaces;
using System.Collections;
using System.Collections.Generic;
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

        public AdministratorController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IAdministratorServices administrator)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _administrator = administrator;
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if(model.Name == null) return BadRequest("Role Not Specified");

            if (await _roleManager.RoleExistsAsync(model.Name)) return BadRequest("Role Already Exists");

            var result = await _administrator.CreateRole(model);
            if (result.Succeeded) return Ok("Role Created Sucessfully");

            return BadRequest("Role Not Created");
        }

        [HttpPost("AddUserToRole")]
        [Authorize("SuperAdmin")]
        public async Task<IActionResult> AddToRole(string userId, [FromBody] RoleModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (model.Name == null) return BadRequest("Role Not Specified");

            var result = _administrator.AddUserToRole(user, model.Name);
            if (!result.IsCompletedSuccessfully) return BadRequest("Assigning Role To User Failed");

            return Ok("Sucessfully Assigned Role To User");
        }

        [HttpGet("GetRoles")]
        public dynamic ViewRoles()
        { 
            return _roleManager.Roles.ToList();
        }

    }
}
