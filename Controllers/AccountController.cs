using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shop.Models.Response;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountServices _accountServices;
        private IAdministratorServices _administrator;
        private IConfiguration _configuration;
        private readonly dynamic data;
        public AccountController(IAccountServices accountServices, IConfiguration configuration, IAdministratorServices administrator)
        {
            _accountServices = accountServices;
            _configuration = configuration;
            _administrator = administrator;

            data = _administrator.GetUserClaims();
        }
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _accountServices.AdminRegistrationAsync(model);

            if (result != null) return Ok("Admin Registration Sucessful");
            return BadRequest(result.Errors);
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> CreateUser([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _accountServices.UserRegistrationAsync(model);

            if (result != null) return Ok("User Registration Sucessful");
            return BadRequest(result.Errors);
        }

        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLoginAsync([FromBody] LoginModel model)
        {

            if (!ModelState.IsValid) return BadRequest("Invalid Properties");

            var results = await _accountServices.LoginAsync(model);

            //---------------------------------------------------------------------------------------//

            if (results.IsSuccess)
            {
                var token = results.Data.ToString();
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

                SecurityToken validatedToken;
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["AuthSettings:Issuer"],
                    ValidAudience = _configuration["AuthSettings:Audience"],
                    IssuerSigningKey = key
                };

                try
                {   
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                    var identity = principal.Identity as ClaimsIdentity;

                    var userRole = identity?.FindFirst(ClaimTypes.Role)?.Value;
                    if (userRole == "User") return Ok(results);
                    return BadRequest("Invalid access for your role");
                }

                catch (Exception)
                {
                    return BadRequest("Invalid Token");
                }
            }
            //---------------------------------------------------------------------------------------//
            return BadRequest(results);
        }

        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Properties");
            var results = await _accountServices.LoginAsync(model);
            
            if (results.IsSuccess)
            {
                var token = results.Data.ToString();
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
                
                SecurityToken validatedToken;
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["AuthSettings:Issuer"],
                    ValidAudience = _configuration["AuthSettings:Audience"],
                    IssuerSigningKey = key
                };

                try
                {   
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                    var identity = principal.Identity as ClaimsIdentity;

                    var userRole = identity?.FindFirst(ClaimTypes.Role)?.Value;
                    var isAllowed = identity?.FindFirst("isAllowed")?.Value;
                    if (userRole == "Admin")
                    {
                        if(isAllowed == "True") return Ok(results);
                        return BadRequest("You are Not Authorized. Please Contact Administrator");
                    }
                    return BadRequest("Invalid access for your role");
                }

                catch (Exception)
                {
                    return BadRequest("Invalid Token");
                }
            }
            return BadRequest(results);
        }

        [HttpPost("SuperAdminLogin")]
        public async Task<IActionResult> SuperAdminLoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Properties");

            var results = await _accountServices.LoginAsync(model);
            if (results.IsSuccess)
            {
                var token = results.Data.ToString();
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
                
                SecurityToken validatedToken;
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["AuthSettings:Issuer"],
                    ValidAudience = _configuration["AuthSettings:Audience"],
                    IssuerSigningKey = key
                };

                try
                {
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                    var identity = principal.Identity as ClaimsIdentity;

                    var userRole = identity?.FindFirst(ClaimTypes.Role)?.Value;
                    if (userRole == "SuperAdmin") return Ok(results);
                    return BadRequest("Invalid access for your role");
                }

                catch (Exception)
                {
                    return BadRequest("Invalid Token");
                }
            }

            return BadRequest(results);
        }

        [HttpPost("EditPermission")]
        public async Task<IActionResult> SetPermission(string username,bool updateStatus)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Properties");
            if (data == null) return BadRequest("No User Logged In");
            if (data["Roles"] != "SuperAdmin") return BadRequest("Permission Denied!");

            var results = _accountServices.EditPermission(username, updateStatus);

            if (results.Result.IsSuccess)
            {
                return Ok($"Permsiion Modified for username {username}");
            }

            return BadRequest(results.Result.Message);
        }

    }
}