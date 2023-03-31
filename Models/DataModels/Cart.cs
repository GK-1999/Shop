using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models.DataModels
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public int ProductId { get; set; }
        public string? UserName { get; set; }
        public int Quantity { get; set; }
        public Status Status { get; set; }
    }
    public enum Status { Added, Purchased}
}
