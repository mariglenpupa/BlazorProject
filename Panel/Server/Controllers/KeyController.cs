using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panel.Server.Services;
using Panel.Shared.Classes;
using System.Security.Claims;

namespace Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Mod")]
    public class KeyController : ControllerBase
    {
        private readonly IRepo _repo;
        public KeyController(IRepo repo) => _repo = repo;

        [HttpGet("GetKeys")]
        public async Task<Key[]> GetKeys()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Admin")
            {
                return await _repo.GetKeys();
            }
            else if (role == "Mod")
            {
                var keys = await _repo.GetKeys();
                return keys.Where(k => k.AddedBy == User.FindFirstValue(ClaimTypes.Name)).ToArray();
            }
            return Array.Empty<Key>();
        }

        [HttpGet("GetKey/{Id:int}")]
        public async Task<Key> GetProduct(int Id)
        {
            return (await _repo.GetKeys()).FirstOrDefault(k => k.Id == Id)!;
        }

        [HttpPost("CreateKey")]
        public async Task<Key> CreateKey(Key key)
        {
            return await _repo.CreateKey(key);
        }

        [Authorize(Roles = "User")]
        [HttpPost("ClaimKey")]
        public async Task<string> ClaimKey(Key key)
        {
            return await _repo.ClaimKey(key);
        }

        [Authorize(Roles = "User")]
        [HttpPost("CheckKeys")]
        public async Task<string> CheckKeys(Key key)
        {
            return await _repo.CheckKeys(key);
        }

        [HttpPost("UpdateKey")]
        public async Task<string> DeleteKey(Key key)
        {
            return await _repo.UpdateKey(key);
        }
    }
}