namespace Shop.Models.ViewModels
{
    public class ViewProducts
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public bool Visiblity { get; set; }
    }
}
