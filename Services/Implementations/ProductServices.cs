using AutoMapper;
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
        private IMapper _mapper;
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
            _dbContext.SaveChanges();
            return new response {Message = "Product Added Sucessfully", StatusCode = 200 };
        }

        public async Task<response> DeleteProductById(int id)
        {
            var product = _dbContext.Products.SingleOrDefault(x => x.ProductId == id);
            if (product == null) return new response { Message = $"No Product Found with of Id : {id}", StatusCode = 400 };

            dynamic data = _administrator.GetUserClaims();
            if (data == null) return new response { Message = "No User Logged In" , StatusCode = 400 };
            if (data.Data.userName != product.UploadedBy) return new response { Message = $"No Such Product Found", StatusCode = 400 };

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return new response { Message = $"{product.Name} Deleted Sucessfully " , StatusCode = 200 };
        }

        public async Task<response> DeleteProductByName(string name)
        {
            var product = _dbContext.Products.SingleOrDefault(x => x.Name == name);
            if (product == null) return new response { Message = $"No Product Found with of Name : {name}", StatusCode = 400 };

            dynamic data = _administrator.GetUserClaims();
            if (data == null) return new response { Message = "No User Logged In", StatusCode = 400 };
            if (data.Data.userName != product.UploadedBy) return new response { Message = $"No Such Product Found", StatusCode = 400 };

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return new response { Message = $"Product Sucessfully Deleted ", StatusCode = 200 };
        }

        public async Task<response> UpdateProduct(UpdateProduct model)
        {
            dynamic data = _administrator.GetUserClaims();
            if (data == null) return new response { Message = "No User Logged In", StatusCode = 400 };

            var product = _dbContext.Products.FirstOrDefault(x => x.ProductId == model.ProductId);
            if (product == null) return new response { Message = "Product Not Found", StatusCode = 400 };

            if (data.Data.uesrName != product.UploadedBy) return new response { Message = "Invalid Operation", StatusCode = 400 };

            if (model.Name != null) product.Name = model.Name;
            if (model.Description != null) product.Description = model.Description ;
            if (model.Category != null) product.Category = model.Category;
            if (model.Price != null) product.Price = model.Price;
            if (model.Quantity != null) product.Quantity = model.Quantity;

            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();

            return new response {Message = "Product Updated Sucessfully" ,StatusCode = 200};
        }

        public async Task<response> ViewAllProducts()
        {
            dynamic data = _administrator.GetUserClaims();
            if (data.StatusCode == 400) return new response { Message = data.Message };
            var role = data.Data.userRole;
            string? userName = data.Data.userName;
            dynamic products = new ViewProducts();

            if (role == "SuperAdmin") products = _dbContext.Products.ToList();
            if (role == "Admin") products = _dbContext.Products.Where(x => x.UploadedBy == userName).ToListAsync();

            //---------------------------------------------------------------------------------//
            if(products.Result != null) return new response { StatusCode = 200,Data = new { products } };
            //---------------------------------------------------------------------------------//
            return new response { Message = "No Products Found" , StatusCode = 400, Data = new { products } };
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
