using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shop.Models.DataModels;
using Shop.Models.Response;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;
using System.Collections.Generic;

namespace Shop.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly IProductServices _product;
        public ProductController(IProductServices product)
        {
            _product = product;
        }

        [HttpPost("AddProduct")]
        public IActionResult addProduct([FromBody] IList<Product> model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Operation");
            var flag = false;
            dynamic result = "";
            var count = 0;
            List<string> ProductStatus = new List<string>();

            foreach (var product in model)
            {
                result = _product.AddProduct(product);
                count += 1;
                ProductStatus.Add($"{count} : {result.Result.Message}");
            }
            if (count > 0) return Ok(ProductStatus);
            return BadRequest("Products Not Added");
        }

        [HttpPut("UpdateProduct")]
        public IActionResult updateProduct([FromBody] UpdateProduct model) 
        {
            if(!ModelState.IsValid) return BadRequest(" Invalid Operation");

            var product = _product.UpdateProduct(model);

            if (product != null) return BadRequest("Product Not Updated");

            return Ok(model);
        }

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            var products = _product.ViewAllProducts(); 

            if(products.IsCompletedSuccessfully) return Ok(products.Result.Data);

            return BadRequest(products.Result);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(IList<CartItem> model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Operation");
            dynamic result = "";
            var count = 0;
            List<string> ItemStatus = new List<string>();

            foreach (var item in model)
            {
                result = _product.AddItems(item);
                count += 1;
                ItemStatus.Add($"{count} : {result.Result.Message}");
            }
            if (count > 0) return Ok(ItemStatus);
            return BadRequest(result.Result);
        }

        [HttpPost("DeleteItemFromCart")]
        public IActionResult RemoveFromCart(int id)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Operation");
            var result = _product.DeleteFromCart(id);
            if (result.Result.IsSuccess) return Ok(result.Result.Data);
            return BadRequest(result.Result);
        }
        
        [HttpGet("GetCart")]
        public IActionResult GetCart()
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Operation");
            var result = _product.UserCart();
            if (result.Result.IsSuccess) return Ok(result.Result.Data);
            return BadRequest(result.Result);
        }

        [HttpPost("Purchase")]
        public IActionResult PurchaseProducts()
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Operation");
            var result = _product.Purchase();
            if (result.Result.IsSuccess) return Ok(result.Result.Data);
            return BadRequest(result.Result);
        }
    }
}
