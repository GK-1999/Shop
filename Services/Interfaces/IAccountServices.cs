using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Response;
using Shop.Models.ViewModels;

namespace Shop.Services.Interfaces
{
    public interface IAccountServices
    {
        Task<IdentityResult> AdminRegistrationAsync([FromBody]RegistrationModel model);
        Task<IdentityResult> UserRegistrationAsync([FromBody] RegistrationModel model);

        Task<response> LoginAsync(LoginModel model);

        Task<response> EditPermission(string username,bool updateStatus);
    }
}
