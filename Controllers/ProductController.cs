using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shop.Models.DataModels;
using Shop.Models.ViewModels;
using Shop.Services.Interfaces;

namespace Shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly IProductServices _product;
        IMapper _mapper;
        public ProductController(IMapper mapper, IProductServices product)
        {
            _mapper = mapper;
            _product = product;
        }

        [HttpPut("UpdateProduct")]
        public IActionResult updateProduct([FromBody] UpdateProduct model) 
        {
            if(!ModelState.IsValid) return BadRequest(" Invalid Operation");

            var product = _product.UpdateProduct(model);

            if (product != null) return BadRequest("Product Not Updated");

            return Ok(model);
        }

        [HttpPost("AddProduct")]
        public IActionResult addProduct([FromBody] Product model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Operation");
            
            var product = _product.AddProduct(model);
            if (product.IsCompleted) return Ok(product.Result.Message);

            return BadRequest("Product Not Added");
        }

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts([FromBody] Product model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Operation");
            var products = _product.ViewAllProducts(); 

            if(products.IsCompletedSuccessfully) return Ok(products.Result.Data);

            return BadRequest(products.Result.Message);
        }
    }
}
