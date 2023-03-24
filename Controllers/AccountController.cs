using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public AccountController(IAccountServices accountServices, IConfiguration configuration, IAdministratorServices administrator)
        {
            _accountServices = accountServices;
            _configuration = configuration;
            _administrator = administrator;
        }
        [HttpPost("RegisterAdmin")]
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
            return BadRequest(result);
        }

        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLoginAsync([FromBody] LoginModel model)
        {

            if (!ModelState.IsValid) return BadRequest("Invalid Properties");

            var results = await _accountServices.LoginAsync(model);

            //---------------------------------------------------------------------------------------//

            if (results.StatusCode == 200)
            {
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
                    dynamic receivedResponse = results.Data;
                    var token = receivedResponse.token.ToString();
                    
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

            //---------------------------------------------------------------------------------------//

            if (results.StatusCode == 200)
            {
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
                    dynamic receivedResponse = results.Data;
                    var token = receivedResponse.token.ToString();
                    
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                    var identity = principal.Identity as ClaimsIdentity;

                    var userRole = identity?.FindFirst(ClaimTypes.Role)?.Value;
                    if (userRole == "Admin") return Ok(results);
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

        [HttpPost("SuperAdminLogin")]
        public async Task<IActionResult> SuperAdminLoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Properties");

            var results = await _accountServices.LoginAsync(model);

            if (results.StatusCode == 200)
            {
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
                    dynamic receivedResponse = results.Data;
                    var token = receivedResponse.token.ToString();
                    
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
    }
}