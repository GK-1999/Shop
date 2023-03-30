using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shop.Models.DataModels;
using Shop.Models.Response;
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
        public IActionResult addProduct([FromBody] IList<Product> model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid Operation");
            var flag = false;
            dynamic result = "";

            foreach (var product in model)
            {
                result = _product.AddProduct(product);
                if (result.Result.IsSuccess) flag = true;
                if (!flag) break;
            }
            if(flag) return Ok(result.Result);

            return BadRequest("Product Not Added");
        }

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            var products = _product.ViewAllProducts(); 

            if(products.IsCompletedSuccessfully) return Ok(products.Result.Data);

            return BadRequest(products.Result);
        }
    }
}
