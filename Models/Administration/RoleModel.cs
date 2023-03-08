using System.ComponentModel.DataAnnotations;

namespace Shop.Models.Administration
{
    public class RoleModel
    {
        [Required]
        public string? Name { get; set; }
    }
}
