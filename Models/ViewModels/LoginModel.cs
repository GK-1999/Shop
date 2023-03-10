using System.ComponentModel.DataAnnotations;

namespace Shop.Models.ViewModels
{
    public class LoginModel
    {
        public string? UserName { get; set; }
        public string? EmailId { get; set; }
        public string? Password { get; set; }
    }
}
