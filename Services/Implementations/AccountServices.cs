using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shop.Models.DataModels;
using Shop.Models.Response;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shop.Services.Implementations
{
    public class AccountServices : IAccountServices
    {
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;

        public AccountServices(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<response> AdminRegistrationAsync([FromBody] RegistrationModel model)
        {
            if (model == null) return new response { Message = "No Data Received", StatusCode = 400 };

            UserDetails IdentityUser = new UserDetails
            {
                UserName = model.UserName,
                Email = model.EmailId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                State = model.State,
                PhoneNumber = model.PhoneNumber,
                Pincode = model.Pincode,
            };
            var result = await _userManager.CreateAsync(IdentityUser, model.Password);
            if (result.Succeeded)
            {
                var Assignrole = await _userManager.AddToRoleAsync(IdentityUser, "Admin");

                if (Assignrole.Succeeded) return new response { Message = "Admin Registration Sucessful", StatusCode = 200 };
            }
            return new response { Message = "Admin Registration Failed", StatusCode = 400 };
        }

        public async Task<response> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return new response { Message = "No Valid User Found with that UserName", StatusCode = 400};

            var valid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!valid) return new response { Message = "Invalid Password", StatusCode = 400 };
            IList<string> roles = await _userManager.GetRolesAsync(user);
            var claims = new[]{
                new Claim("UserName" , model.UserName),
                new Claim(ClaimTypes.NameIdentifier , user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
            
            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new response
            {
                Message = "Login Sucessful",
                StatusCode = 200,
                Data = new { token = tokenAsString ,UserRole = roles.FirstOrDefault() },
            };
        }

        public async Task<response> UserRegistrationAsync([FromBody] RegistrationModel model)
        {
            if (model == null) return new response { Message = "No Data Received", StatusCode = 400 };

            var userExists = _userManager.Users.Any(u => u.UserName == model.UserName);
            if (userExists) return new response { Message = "User Already Exists", StatusCode = 400 };
            var emailExists = _userManager.Users.Any(u => u.Email == model.EmailId);
            if (emailExists) return new response { Message = "User with this EmailId Already Exists", StatusCode = 400 };

            UserDetails IdentityUser = new UserDetails
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Email = model.EmailId,
                PhoneNumber = model.PhoneNumber,
                Pincode = model.Pincode,
                IsAllowed = true,
            };

            var result = await _userManager.CreateAsync(IdentityUser, model.Password);
            if (result.Succeeded)
            {
                var Assignrole = await _userManager.AddToRoleAsync(IdentityUser, "User");
                if (Assignrole.Succeeded) return new response { Message = "User Registration Sucessful", StatusCode = 200 };
            }
            return new response { Message = "User Registration Failed", StatusCode = 400 };
        }
    }
}
