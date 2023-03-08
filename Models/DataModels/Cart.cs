using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models.DataModels
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("Id")]
        public string? UserId { get; set; }
        public int Quantity { get; set; }
        public Status Status { get; set; }
    }
    public enum Status { Added, Purchased }
}
