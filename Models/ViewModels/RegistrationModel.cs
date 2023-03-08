using System.ComponentModel.DataAnnotations;

namespace Shop.Models.ViewModels
{
    public class RegistrationModel
    {
       
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? EmailId { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int Pincode { get; set; }
    }
}
