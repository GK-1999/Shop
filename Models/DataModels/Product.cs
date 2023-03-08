namespace Shop.Models.DataModels
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set;}
        public bool Visiblity { get; set; }
        public string UploadedBy { get; set; }
    }
}
