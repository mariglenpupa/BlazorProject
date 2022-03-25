using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panel.Server.Services;
using Panel.Shared.Classes;

namespace Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Mod")]
    public class UserController : ControllerBase
    {
        private readonly IRepo _repo;
        public UserController(IRepo repo) => _repo = repo;

        [HttpGet]
        public async Task<User[]> GetUsers()
        {
            return await _repo.GetUsers();
        }

        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<string> LogIn(User user)
        {
            return await _repo.ManageUser(user, Request.HttpContext.Connection.RemoteIpAddress!.ToString(), false);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<string> Register(User user)
        {
            return await _repo.ManageUser(user, Request.HttpContext.Connection.RemoteIpAddress!.ToString(), true);
        }

        [HttpPost("UpdateUser")]
        public async Task<string> UpdateUser(User user)
        {
            return await _repo.UpdateUser(user);
        }

        [HttpGet("GetUser/{UUID}")]
        public async Task<User> GetProduct(string UUID)
        {
            return (await _repo.GetUsers()).FirstOrDefault(u => u.UUID == UUID)!;
        }
    }
}