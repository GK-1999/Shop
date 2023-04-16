using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DbContext;
using Shop.Models.DataModels;
using Shop.Models.Response;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Xml.Linq;

namespace Shop.Services.Implementations
{
    public class ProductServices : IProductServices
    {
        private ShopDbContext _dbContext;
        private IAdministratorServices _administrator;
        dynamic loginDetails;
        public ProductServices(ShopDbContext dbContext, IAdministratorServices administrator)
        {
            _dbContext = dbContext;
            _administrator = administrator;
            loginDetails = _administrator.GetUserClaims();
        }
        public async Task<response> AddProduct(Product model)
        {
            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };

            var role = loginDetails["Roles"];
            var name = loginDetails["username"];

            if (role != "Admin" && role != "SuperAdmin") return new response { Message = "User Not Allowed for this action", IsSuccess = false };

            var product = _dbContext.Products.FirstOrDefault(x => x.Name == model.Name);
            if (product != null) { return new response { Message = $"{model.Name} is Already Present", IsSuccess = false }; }

            model.UploadedBy = name;
            _dbContext.Products.Add(model);
            _dbContext.SaveChanges();

            return new response { Message = $"{model.Name} Added Sucessfully", IsSuccess = true };
        }

        public async Task<response> AddItems(CartItem cartItems)
        {
            if (cartItems == null) return new response { Message = "No Data Received", IsSuccess = false };

            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };

            string name = loginDetails["username"];

            var product = _dbContext.Products.FirstOrDefault(x => x.Name == cartItems.ProductName);
            if (product == null) return new response { Message = "No Such Product Found", IsSuccess = false };

            var cartProduct = _dbContext.Cart.FirstOrDefault(x =>
            (x.ProductName == cartItems.ProductName) && (x.UserName == name));
            if (cartProduct != null) { return new response { Message = $"{cartItems.ProductName} is Already Present in Your Cart", IsSuccess = false }; }

            var addItem = new Cart();

            addItem.UserName = name;
            addItem.ProductName = product.Name;
            addItem.ProductId = product.ProductId;
            addItem.Quantity = cartItems.Quantity;
            addItem.Price = product.Price;
            addItem.Status = Status.Added;

            _dbContext.Cart.Add(addItem);
            _dbContext.SaveChanges();
            return new response { Message = $"{cartItems.ProductName} Added Sucessfully in Your Cart ", IsSuccess = true };
        }

        public async Task<response> UserCart()
        {
            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };

            string name = loginDetails["username"];

            var cartProduct = _dbContext.Cart.Where(x => x.UserName == name).ToList();
            if (cartProduct.Capacity > 0) return new response { IsSuccess = true, Data = cartProduct };

            return new response { Message = "Cart is Empty", IsSuccess = false };
        }

        public async Task<response> DeleteProductById(int id)
        {
            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };

            var product = _dbContext.Products.SingleOrDefault(x => x.ProductId == id);
            if (product == null) return new response { Message = $"No Product Found with of Id : {id}", IsSuccess = false };

            if (loginDetails["username"] != product.UploadedBy) return new response { Message = $"No Such Product Found", IsSuccess = false };

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return new response { Message = $"{product.Name} Deleted Sucessfully ", IsSuccess = true };
        }

        public async Task<response> DeleteProductByName(string name)
        {
            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };
            var product = _dbContext.Products.SingleOrDefault(x => x.Name == name);
            if (product == null) return new response { Message = $"No Product Found with of Name : {name}", IsSuccess = false };

            if (loginDetails["username"] != product.UploadedBy) return new response { Message = $"No Such Product Found", IsSuccess = false };

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return new response { Message = $"Product Sucessfully Deleted ", IsSuccess = true };
        }

        public async Task<response> UpdateProduct(UpdateProduct model)
        {
            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };

            var product = _dbContext.Products.FirstOrDefault(x => x.ProductId == model.ProductId);
            if (product == null) return new response { Message = "Product Not Found", IsSuccess = false };

            if (loginDetails["username"] != product.UploadedBy) return new response { Message = "Invalid Operation", IsSuccess = false };

            if (model.Name != null) product.Name = model.Name;
            if (model.Description != null) product.Description = model.Description;
            if (model.Category != null) product.Category = model.Category;
            if (model.Price != null) product.Price = model.Price;
            if (model.Quantity != null) product.Quantity = model.Quantity;

            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();

            return new response { Message = "Product Updated Sucessfully", IsSuccess = true };
        }

        public async Task<response> ViewAllProducts()
        {
            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };

            var role = loginDetails["Roles"];
            string userName = loginDetails["username"];

            dynamic products = new ViewProducts();

            if (role == "SuperAdmin") products = _dbContext.Products.ToList();
            if (role == "Admin") products = _dbContext.Products.Where(x => x.UploadedBy == userName).ToListAsync();

            //---------------------------------------------------------------------------------//
            if (products != null) return new response { IsSuccess = true, Data = new { products } };
            //---------------------------------------------------------------------------------//
            return new response { Message = "No Products Found", IsSuccess = false, Data = new { products } };
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

        public dynamic ViewProductById(int Id)
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
            product.Visiblity = productDetails.Visiblity;

            return product;
        }

        public dynamic ViewProductByName(string name)
        {
            var products = _dbContext.Products.Where(x => x.Name == name).ToListAsync();
            if (products == null) return new response { Message = "No Product Found with this name" };
            return (IEnumerable<ViewProducts>)products;
        }

        public async Task<response> DeleteFromCart(int id)
        {
            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };
            string username = loginDetails["username"];
            var item = _dbContext.Cart.FirstOrDefault(x => (x.ProductId == id) && (x.UserName == username));
            if (item == null) return new response { Message = $"No Product Found with of Id : {id}", IsSuccess = false };

            _dbContext.Cart.Remove(item);
            _dbContext.SaveChanges();

            return new response { Message = $"{item.ProductName} Removed From Cart", IsSuccess = true };
        }

        public async Task<response> Purchase()
        {
            if (loginDetails == null) return new response { Message = "No user Logged In", IsSuccess = false };

            string name = loginDetails["username"];

            var product = _dbContext.Cart.Where(x => x.UserName == name).ToList();
            if (product.Count == 0) return new response { Message = "No Products Found", IsSuccess = false };

            var lastOrderId = _dbContext.ProductPurchaseHistory.OrderByDescending(x => x.OrderId).Select(x => x.OrderId).FirstOrDefault();
            int orderId = lastOrderId + 1;

            List<ProductPurchaseHistory> itemList = new List<ProductPurchaseHistory>();

            foreach (var item in product)
            {
                var Prd = _dbContext.Products.SingleOrDefault(x => x.ProductId == item.ProductId);

                if (item.Quantity <= Prd.Quantity)
                {
                    ProductPurchaseHistory PurchaseItem = new ProductPurchaseHistory();
                    PurchaseItem.OrderId = orderId;
                    PurchaseItem.ProductId = item.ProductId;
                    PurchaseItem.ProductName = item.ProductName;
                    PurchaseItem.OrderDate = DateTime.Now;
                    PurchaseItem.Price = item.Price;
                    PurchaseItem.Quantity = item.Quantity;
                    PurchaseItem.TotalPrice = PurchaseItem.Price * PurchaseItem.Quantity;
                    Prd.Quantity -= PurchaseItem.Quantity;

                    _dbContext.ProductPurchaseHistory.Add(PurchaseItem);
                    _dbContext.Products.Update(Prd);
                    _dbContext.Cart.Remove(item);
                    _dbContext.SaveChanges();

                    itemList.Add(PurchaseItem);
                }
            }
            return new response { Data = itemList, IsSuccess = true };
        }
    }
}
