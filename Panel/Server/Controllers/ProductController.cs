using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panel.Server.Services;
using Panel.Shared.Classes;

namespace Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IRepo _repo;
        public ProductController(IRepo repo) => _repo = repo;

        [HttpGet("GetProducts")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<ActionResult<Product[]>> GetProducts()
        {
            return await _repo.GetProducts();
        }

        [HttpGet("GetProduct/{Id:int}")]
        [Authorize(Roles = "Admin, Mod")]
        public async Task<Product> GetProduct(int Id)
        {
            return (await _repo.GetProducts()).FirstOrDefault( p => p.Id == Id)!;
        }


        [HttpPost("CreateProduct")]
        public async Task<Product> CreateProduct(Product product)
        {
            return await _repo.CreateProduct(product);
        }

        [HttpPost("UpdateProduct")]
        public async Task<string> UpdateProduct(Product product)
        {
            return await _repo.UpdateProduct(product);
        }
    }
}
