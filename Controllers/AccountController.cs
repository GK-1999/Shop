using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Administration;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountServices _accountServices;
        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }
        [HttpPost("CreateAdmin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _accountServices.AdminRegistrationAsync(model);

            if (result.StatusCode == 200) return Ok(result.Message);
            return BadRequest(result.Message);
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> CreateUser([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _accountServices.UserRegistrationAsync(model);

            if (result.StatusCode == 200) return Ok(result.Message);
            return BadRequest(result.Message);
        }

        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Properties");

            var results = await _accountServices.LoginAsync(model);

            if (results.StatusCode == 200)
            {
                dynamic receivedResponse = results.Data;
                dynamic userRole = receivedResponse.UserRole.ToString();
                if (userRole == "User") return Ok(receivedResponse.token.ToString());
                return BadRequest("Invalid access for your role");
            }
            return BadRequest(results.Message);
        }

        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Properties");

            var results = await _accountServices.LoginAsync(model);

            if (results.StatusCode == 200)
            {
                dynamic receivedResponse = results.Data;
                dynamic userRole = receivedResponse.UserRole.ToString();
                if (userRole == "Admin") return Ok(receivedResponse.token.ToString());
                return BadRequest("Invalid access for your role");
            }
            return BadRequest(results.Message);
        }
    }
}