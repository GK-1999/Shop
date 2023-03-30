using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shop.DbContext;
using Shop.Models.DataModels;
using Shop.Models.Response;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Shop.Services.Implementations
{
    public class AccountServices : IAccountServices
    {
        private UserManager<IdentityUser> _userManager;
        private ShopDbContext _dbContext;
        private IConfiguration _configuration;

        public AccountServices(UserManager<IdentityUser> userManager,
                               IConfiguration configuration, ShopDbContext dbContext,IAdministratorServices administrator)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<IdentityResult?> AdminRegistrationAsync([FromBody] RegistrationModel model)
        {
            if (model == null) return null;

            var userExists = _userManager.Users.Any(u => u.UserName == model.UserName);
            if (userExists) return null;
            var emailExists = _userManager.Users.Any(u => u.Email == model.EmailId);
            if (emailExists) return null;

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
                if (Assignrole.Succeeded) return result;
            }
            return (IdentityResult?)result.Errors;
        }

        public async Task<IdentityResult?> UserRegistrationAsync([FromBody] RegistrationModel model)
        {
            if (model == null) return null;

            var userExists = _userManager.Users.Any(u => u.UserName == model.UserName);
            if (userExists) return null;
            var emailExists = _userManager.Users.Any(u => u.Email == model.EmailId);
            if (emailExists) return null;

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
                if (Assignrole.Succeeded) return result;
            }
            return (IdentityResult?)result.Errors;
        }

        public async Task<response> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return new response { Message = "Invalid Username / EmailId", IsSuccess = false };

            UserDetails details = (UserDetails)user;

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
            UserDetails userProperties = (UserDetails)user;

            if (userProperties.IsAllowed != updateStatus)
            {
                userProperties.IsAllowed = updateStatus;

                var result = await _userManager.UpdateAsync(userProperties);

                if (result.Succeeded) return new response { Message = "Sucess", IsSuccess = true, Data = result };
                return new response{ Message = "Failed", IsSuccess = false, Data = result.Errors };
            }
            return new response
            {
                Message = $"{username} already have the status you are trying to update",
                IsSuccess = false
            };
        }
    }
}