using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models.DataModels
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public double Price { get; set; }
        public int Quantity { get; set; }
        public bool Visiblity { get; set; } 
        public string? UploadedBy { get; set; }
    }
}
