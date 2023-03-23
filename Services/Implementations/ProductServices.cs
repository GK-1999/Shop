using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DbContext;
using Shop.Models.DataModels;
using Shop.Models.Response;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;
using System.Xml.Linq;

namespace Shop.Services.Implementations
{
    public class ProductServices : IProductServices
    {
        private ShopDbContext _dbContext;
        private IAdministratorServices _administrator; 
        public ProductServices(ShopDbContext dbContext, IAdministratorServices administrator)
        {
            _dbContext = dbContext;
            _administrator = administrator;
        }
        public async Task<response> AddProduct(Product model)
        {
            var loginDetails = _administrator.GetUserClaims();
            dynamic data = loginDetails.Data;
            var role = data.userRole;
            var name = data.userName;

            if(role != "Admin" && role != "SuperAdmin") return new response { Message = "User Not Allowed for this action", StatusCode = 400};

            model.UploadedBy = name;
            _dbContext.Products.Add(model);
            //_dbContext.SaveChanges();
            return new response {Message = "Product Added Sucessfully", StatusCode = 200 };
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

        public async Task<response> UpdateProduct(UpdateProduct model)
        {
            var loginDetails = _administrator.GetUserClaims();
            dynamic data = loginDetails.Data;
            var name = data.userName;

            var product = _dbContext.Products.FirstOrDefault(x => x.ProductId == model.ProductId);


            if (product.UploadedBy != name) return new response { Message = "Invalid Operation", StatusCode = 400 };
            if (product == null) return new response { Message = "Product Not Found", StatusCode = 400 };
            
            if (model.Name == null) model.Name = product.Name;
            if (model.Description == null) model.Description = product.Description;
            if (model.Category == null) model.Category = product.Category;
            if (model.Price == null) model.Price = product.Price;
            if( model.Quantity == null) model.Quantity = product.Quantity;


            _dbContext.Products.Update(model);
            _dbContext.SaveChanges();

            return new response {Message = "Product Updated Sucessfully" ,StatusCode = 200};
        }

        public IEnumerable<ViewProducts> ViewAllProducts()
        {
            var products = _dbContext.Products.ToList();
            return (IEnumerable<ViewProducts>)products;
        }

        public dynamic ViewProductByAdmin(string name)
        {
            var products = _dbContext.Products.Where(x => x.UploadedBy == name).ToListAsync();
            if (products == null) return new response { Message = "No Product Found for this Admin" };
            return (IEnumerable<ViewProducts>)products;
        }

        public dynamic ViewProductByCategory(string category)
        {
            var products = _dbContext.Products.Where(x => x.Category == category).ToListAsync();
            if (products == null) return new response { Message = "No Product Found with this name" };
            return (IEnumerable<ViewProducts>)products;
        }

        public  dynamic ViewProductById(int Id)
        {
            var productDetails = _dbContext.Products.FirstOrDefault(x => x.ProductId == Id);

            if (productDetails == null) return new response { Message = "No Product Found with this id" };

            ViewProducts product = new ViewProducts();
            product.ProductId = productDetails.ProductId;
            product.Name = productDetails.Name;
            product.Description = productDetails.Description;
            product.Category = productDetails.Category;
            product.Price = productDetails.Price;
            product.Quantity = productDetails.Quantity;
            product.Visiblity= productDetails.Visiblity;

            return product;
        }

        public dynamic ViewProductByName(string name)
        {
            var products = _dbContext.Products.Where(x => x.Name == name).ToListAsync();
            if (products == null) return new response { Message = "No Product Found with this name" };
            return (IEnumerable<ViewProducts>)products;            
        }
    }
}
