using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panel.Server.Services;
using Panel.Shared.Classes;

namespace Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Mod")]
    public class InfoController : Controller
    {
        readonly IRepo _repo;
        public InfoController(IRepo repo) => _repo = repo;


        [HttpGet("GetSales")]
        public async Task<List<Stat>> GetSales()
        {
            var products = await _repo.GetProducts();
            var keys = await _repo.GetKeys();
            List<Key> validKeys = new();
            List<Stat> stats = new();

            // Remove Invalid Keys
            foreach(var key in keys)
            {
                if (!key.IsValid)
                    continue;
                if (key.StartDate == DateTime.MinValue)
                    continue;

                validKeys.Add(key);
            }

            foreach(var product in products)
            {
                var count = validKeys.Where(k => k.ProductName == product.Name).ToArray().Length;
                stats.Add(new Stat { PoructName = product.Name, Count = count });
            }

            return stats;
        }
    }
}
