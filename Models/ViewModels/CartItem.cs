using System.ComponentModel.DataAnnotations;

namespace Shop.Models.ViewModels
{
    public class CartItem
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
    }
}

