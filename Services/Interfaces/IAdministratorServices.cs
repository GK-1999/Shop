using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Models.Administration;
using Shop.Models.Response;
using System.Security.Claims;
using System.Security.Principal;

namespace Shop.Services.Interfaces
{
    public interface IAdministratorServices
    {
        Task<IdentityResult> CreateRole(RoleModel name);
        Dictionary<string, string> GetUserClaims();
    }
}
