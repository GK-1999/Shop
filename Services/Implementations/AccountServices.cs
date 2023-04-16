using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shop.DbContext;
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

        public AccountServices(UserManager<IdentityUser> userManager,
                               IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<response?> AdminRegistrationAsync([FromBody] RegistrationModel model)
        {
            if (model == null) return new response { Message = "No data Received" , IsSuccess = false};

            var userExists = _userManager.Users.Any(u => u.UserName == model.UserName);
            if (userExists) return new response { Message = "Username already Present , Please try another username", IsSuccess = false };
            var emailExists = _userManager.Users.Any(u => u.Email == model.EmailId);
            if (emailExists) return new response { Message = "User with this EmailId already exists", IsSuccess = false };

            UserDetails user = new UserDetails
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Email = model.EmailId,
                PhoneNumber = model.PhoneNumber,
                Pincode = model.Pincode
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin")) return null;

                var Assignrole = await _userManager.AddToRoleAsync(user, "Admin");
                if (Assignrole.Succeeded) return new response { Message = "Admin Created", IsSuccess = true , Data = result };
            }
            return new response { Message = "Admin Registration Failed", IsSuccess = false, Data = result };
        }

        public async Task<response?> UserRegistrationAsync([FromBody] RegistrationModel model)
        {
            if (model == null) return new response { Message = "No data Received", IsSuccess = false };

            var userExists = _userManager.Users.Any(u => u.UserName == model.UserName);
            if (userExists) return new response { Message = "Username already Present , Please try another username", IsSuccess = false };
            var emailExists = _userManager.Users.Any(u => u.Email == model.EmailId);
            if (emailExists) return new response { Message = "User with this EmailId already exists", IsSuccess = false };

            UserDetails user = new UserDetails
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

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "User")) return null;

                var Assignrole = await _userManager.AddToRoleAsync(user, "User");
                if (Assignrole.Succeeded) return new response { Message = "User Created Sucessfully", IsSuccess = true, Data = result };
            }
            return new response { Message = "User Registration Failed", IsSuccess = false, Data = result };
        }

        public async Task<response> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return new response { Message = "Invalid Username / EmailId", IsSuccess = false };

            UserDetails details = user as UserDetails;

            if (!await _userManager.CheckPasswordAsync(user, model.Password)) return new response{Message = "Invalid Password" , IsSuccess = false};
            var roles = await _userManager.GetRolesAsync(user);
            var isAllowed = details.IsAllowed;
            if(!isAllowed) return new response { Message = "Access Denied ", IsSuccess = false };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role , roles.FirstOrDefault()),
                new Claim("IsAllowed" , details.IsAllowed.ToString())
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

            return new response { Message = "Details Verified",IsSuccess = true ,Data = tokenAsString  };
        }

        public async Task<response> EditPermission(string username,bool updateStatus)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == username);
            UserDetails? userProperties = user as UserDetails;

            if (userProperties.IsAllowed != updateStatus)
            {
                userProperties.IsAllowed = updateStatus;

                var result = await _userManager.UpdateAsync(userProperties);

                if (result.Succeeded) return new response { Message = "Sucess", IsSuccess = true, Data = result };
                return new response{ Message = "Failed", IsSuccess = false, Data = result };
            }
            return new response
            {
                Message = $"{username} already have the status you are trying to update",
                IsSuccess = false
            };
        }
    }
}