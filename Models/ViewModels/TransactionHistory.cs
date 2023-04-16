using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Shop.Models.ViewModels
{
    public class TransactionHistory
    {
        [Required]
        public int OrderID { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentMethod{ get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set;}
        public string TransactionID { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
    }
}
