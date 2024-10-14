using AuthServer.Core.Dtos;
using AuthServer.Core.Entity;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthServer.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly IGenericServices<Product,ProductDto> _services;

        public ProductController(IGenericServices<Product, ProductDto> services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts() 
        {
            return ActionResultInstance(await _services.GetAllAsync());
        }


        [HttpPost]
        //[SwaggerOperation(Summary = "Creates a new product", Description = "Id alanı otomatik olarak oluşturulur ve istemci tarafından gönderilmez.")]
        public async Task<IActionResult> AddProduct(ProductDto product)
        {
            return ActionResultInstance(await _services.AddAsync(product));
        }


        [HttpPut]
        //[SwaggerOperation(Summary = "Updates an existing product", Description = "Id alanı güncelleme işlemi için gereklidir.")]
        public async Task<IActionResult> UpdateProduct(ProductDto product) 
        {
            return ActionResultInstance(await _services.Update(product,product.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return ActionResultInstance(await _services.Remove(id));
        }

    }
}
