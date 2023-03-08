using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models.DataModels
{
    public class PurchaseHistory
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        [ForeignKey("Id")]
        public int UserId { get; set; }
        public int Quantity { get; set; }
    }
}
