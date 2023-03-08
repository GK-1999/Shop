using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models.DataModels
{
    public class UserDetails : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int Pincode { get; set; }
        public bool IsAllowed { get; set; }
    }
}