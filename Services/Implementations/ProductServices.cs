using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.DbContext;
using Shop.Models.DataModels;
using Shop.Models.Response;
using Shop.Services.Interfaces;
using System.Xml.Linq;

namespace Shop.Services.Implementations
{
    public class ProductServices : IProductServices
    {
        ShopDbContext? _dbContext;

        public ProductServices(ShopDbContext? dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<response> AddProduct(Product model)
        {
            throw new NotImplementedException();
        }

        public async Task<response> DeleteProductById(int id)
        {
            var product = _dbContext.Products.SingleOrDefault(x => x.ProductId == id);

            //if (currentUser != product.UploadedBy)
            //return new response { Message = $"No Such Product Found", StatusCode = 400 };

            if (product == null) return new response { Message = $"No Product Found with of Id : {id}", StatusCode = 400 };
            
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return new response { Message = $"Product Sucessfully Deleted " , StatusCode = 200 };
        }

        public async Task<response> DeleteProductByName(string name)
        {
            var product = _dbContext.Products.SingleOrDefault(x => x.Name == name);

            //if (currentUser != product.UploadedBy)
            //return new response { Message = $"No Such Product Found", StatusCode = 400 };

            if (product == null) return new response { Message = $"No Product Found with of Name : {name}", StatusCode = 400 };

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return new response { Message = $"Product Sucessfully Deleted ", StatusCode = 200 };
        }

        public Task<response> UpdateProduct(Product model)
        {

            throw new NotImplementedException();
        }

        public Task<IActionResult> ViewAllProducts()
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> ViewProductByAdmin(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> ViewProductByCategory(string category)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> ViewProductById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> ViewProductByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
