using Microsoft.AspNetCore.Mvc;
using Shop.Models.DataModels;
using Shop.Models.Response;
using Shop.Models.ViewModels;

namespace Shop.Services.Interfaces
{
    public interface IProductServices
    {
        Task<response> AddProduct(Product model);
        Task<response> UpdateProduct(UpdateProduct model);
        Task<response> DeleteProductById(int id);
        Task<response> DeleteProductByName(string name);

        Task<response> ViewAllProducts();
        dynamic ViewProductById(int Id);
        dynamic ViewProductByName(string name);
        dynamic ViewProductByAdmin(string name);
        dynamic ViewProductByCategory(string category);

        Task<response> AddItems(CartItem cartItems);
        Task<response> UserCart();
    }
}
