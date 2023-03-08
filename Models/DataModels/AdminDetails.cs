using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models.DataModels
{
    public class AdminDetails : IdentityUser    
    {
        [Key]
        public override string Id { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public override string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [EmailAddress]
        public string? EmailId { get; set; }
        public bool IsAllowed { get; set; } = false;

    }
}
