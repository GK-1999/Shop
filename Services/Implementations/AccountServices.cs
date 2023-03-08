using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.DataModels;
using Shop.Models.Response;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;

namespace Shop.Services.Implementations
{
    public class AccountServices : IAccountServices
    {
        private UserManager<IdentityUser> _userManager;

        //public AccountServices(UserDetails userDetails)
        //{       
        //    _userDetails = userDetails;
        //}
        public AccountServices(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public Task<ActionResult<string>> AdminLoginAsync(LoginModel model)
        {
            throw new NotImplementedException();
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

        public Task<ActionResult<string>> UserLoginAsync(LoginModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<response> UserRegistrationAsync([FromBody] RegistrationModel model)
        {
            if (model == null) return new response { Message = "No Data Received", StatusCode = 400 };
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
