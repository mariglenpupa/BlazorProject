using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panel.Server.Services;
using Panel.Shared.Classes;

namespace Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Mod")]
    public class FileController : Controller
    {
        readonly ApplicationDbContext _dbContext;
        readonly IWebHostEnvironment _env;
        public FileController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        [Authorize(Roles = "User")]
        [HttpGet("CheckUpdate")]
        public async Task<IActionResult> CheckUpdate(Product product)
        {
            // Check User
            var userName = User.Identity!.Name;
            var user = await _dbContext.Users!.AsNoTracking().Include(u => u.ActiveKeys).FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == default)
                return NotFound("Error: User Not Found!");

            // Check Access
            bool hasAccess = user.ActiveKeys.Where(k => k.ProductName == product.Name).Any();
            if (!hasAccess)
                return Unauthorized("Error: You Don't Own That Product!");

            // Compare
            Version clientVersion, serverVersion;
            if (!Version.TryParse(product.Version, out clientVersion!))
                return NotFound("Error: Couldn't Parse Version!");

            var prod = await _dbContext.Products!.FirstOrDefaultAsync(p => p.Name == product.Name);
            if (!Version.TryParse(prod!.Version, out serverVersion!))
                return NotFound("Error: Couldn't Parse Version!");

            if (serverVersion > clientVersion)
                return Ok("Update Available!");
            else
                return Ok("You Are Up To Date!");
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProduct(Product product)
        {
            // Check User
            var userName = User.Identity!.Name;
            var user = await _dbContext.Users!.AsNoTracking().Include(u => u.ActiveKeys).FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == default)
                return NotFound("Error: User Not Found!");

            // Check Access
            bool hasAccess = user.ActiveKeys.Where(k => k.ProductName == product.Name).Any();
            if (!hasAccess)
                return Unauthorized("Error: You Don't Own That Product!");

            // Send File
            return File(System.IO.File.ReadAllBytes(Path.Combine(Path.Combine("ProductFiles", product.Name + ".zip"))), "Zip Archive");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromForm] IFormFile file)
        {
            try
            {
                // Checks
                if (file.Length > 100 * 1024 * 1024)
                    return NotFound("File Rejected.");
                var name = Path.GetFileNameWithoutExtension(file.FileName);
                var product = await _dbContext.Products!.FirstOrDefaultAsync(p => p.Name == name);
                if (product == default)
                    return NotFound("Error: Product name was not found: " + name);

                // Save file to disk.
                string path = Path.Combine(_env.ContentRootPath, "ProductFiles");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = Path.Combine(_env.ContentRootPath, "ProductFiles", product.Name + ".zip");
                using (var stream = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok("Success!");
            }
            catch (Exception ex)
            {
                return NotFound("Error: " + ex.Message);
            }
        }
    }
}
