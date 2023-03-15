using Microsoft.AspNetCore.Mvc;
using Shop.Models.DataModels;
using Shop.Models.Response;

namespace Shop.Services.Interfaces
{
    public interface IProductServices
    {
        Task<response> AddProduct(Product model);
        Task<response> UpdateProduct(Product model);
        Task<response> DeleteProductById(int id);
        Task<response> DeleteProductByName(string name);

        Task<IActionResult> ViewAllProducts();
        Task<IActionResult> ViewProductById(int Id);
        Task<IActionResult> ViewProductByName(string name);
        Task<IActionResult> ViewProductByAdmin(string name);
        Task<IActionResult> ViewProductByCategory(string category);
    }
}
