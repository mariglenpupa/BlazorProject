using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Panel.Shared.Classes;
using Panel.Shared.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Panel.Server.Services
{
    public class Repo : IRepo
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        public Repo(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // User
        public async Task<string> ManageUser(IUser user, string IP, bool register)
        {
            var _user = await _db.Users!.FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (_user == default)
            {
                if (register)
                    _user = new User
                    {
                        UUID = Guid.NewGuid().ToString(),
                        Role = Role.User,

                        UserName = user.UserName,
                        Password = Shared.UserHelper.HashPass(user.UserName, user.Password),
                        Contact = user.Contact,
                        Joined = DateTime.Now,
                    };
                else
                    return "Error! UserName Not Found!";
            }
            else
            {
                if (register)
                    return "Error! UserName Already Exists. Please Log In Instead!";
                else if (_user.Password != Shared.UserHelper.HashPass(user.UserName, user.Password))
                    return "Error! Incorrect Credentials."; // Password Incorrect.
            }

            // Add HWID - Stored on user.UUID
            if (!string.IsNullOrEmpty(user.UUID) && !_user.MachineList.Contains(user.UUID))
                _user.MachineList += " " + user.UUID;

            // Add IP
            if (!_user.IPList.Contains(IP)) // Using User UUID to store HWID
                _user.IPList += " " + IP;

            if (register)
            {
                await _db.Users!.AddAsync(_user);
                await _db.SaveChangesAsync();
            }

            return GetToken(_user);
        }
        public async Task<User> GetUser(string userToken)
        {
            var claim = Shared.TokenHelper.ParseClaimsFromJwt(userToken).FirstOrDefault();
            if (claim == default)
                return new User();

            var userName = claim.Value;
            var user = await _db.Users!.FirstOrDefaultAsync(u => u.UserName == userName);

            return user!;
        }
        public string GetToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:TokenKey").Value));
            var token = new JwtSecurityToken(
                null,
                null,
                new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                },
                expires: DateTime.Now.AddMonths(1), // Token valid 4 1 month.
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature));
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        public async Task<User[]> GetUsers()
        {
            return await _db.Users!.ToArrayAsync();
        }
        public async Task<string> UpdateUser(IUser user)
        {
            var _user = await _db.Users!.FirstOrDefaultAsync(u => u.UUID == user.UUID);
            if (_user == default)
                return "Error!";

            _user.UserName = user.UserName;
            _user.Role = user.Role;
            _db.Users!.Update(_user);
            await _db.SaveChangesAsync();

            return "Success!";
        }

        // Key
        public async Task<Key> CreateKey(IKey key)
        {
            var userToken = key.AddedBy;
            var user = await GetUser(userToken);
            var _key = await _db.Keys!.FirstOrDefaultAsync(k => k.ProductKey == key.ProductKey);

            if (string.IsNullOrEmpty(userToken))
                return new Key();

            if (user == null)
                return new Key();

            if (user.Role < Role.Mod)
                return new Key();

            if (_key != default) // Key already exists
                return new Key();

            Key k = new()
            {
                ProductKey = key.ProductKey,
                AddedBy = user.UserName,
                LicenseLength = key.LicenseLength,
                ProductName = key.ProductName // Could have used Id by anyway.
            };

            await _db.Keys!.AddAsync(k!);
            await _db.SaveChangesAsync();
            return k;
        }
        public async Task<string> ClaimKey(IKey key)
        {
            var userToken = key.AddedBy;
            if (string.IsNullOrEmpty(userToken))
                return "Error!";

            var user = await GetUser(userToken);
            if (user == default)
                return "Error!";

            var _key = await _db.Keys!.FirstOrDefaultAsync(k => k.ProductKey == key.ProductKey);
            user.ActiveKeys.Add(_key!);
            _key!.User = user;
            _key.StartDate = DateTime.Now;

            _db.Keys!.Update(_key);
            _db.Users!.Update(user);
            await _db.SaveChangesAsync();
            return "Success!";
        }
        // Neet update. we get usertoken from controller. get user, then check if he has keys...?
        // We also need the prouct tho... :| hmmm
        public async Task<string> CheckKeys(IKey key)
        {
            var userToken = key.AddedBy;
            if (string.IsNullOrEmpty(userToken))
                return "Error!";

            var user = await GetUser(userToken);
            if (user == null)
                return "Error!";

            var keys = await _db.Keys!.Where(k => k.User == user && k.ProductName == key.ProductName).ToListAsync();
            if (keys != null && keys.Count > 0)
            {
                foreach (var _key in keys)
                {
                    if (_key.IsValid)
                        return Guid.NewGuid().ToString(); //Ok("Valid");
                }
            }
            return "Error";
        }
        public async Task<Key[]> GetKeys()
        {
            return await _db.Keys!.ToArrayAsync();
        }
        public async Task<string> UpdateKey(IKey key)
        {
            var _key = await _db.Keys!.FirstOrDefaultAsync(k => k.ProductKey == key.ProductKey);
            if (_key == default)
                return "Error!";

            if (key.LicenseLength == TimeSpan.Zero)
            {
                _db.Keys!.Remove(_key);
            }
            else
            {
                _key.LicenseLength = key.LicenseLength;
                _key.Package = key.Package;
                _db.Keys!.Update(_key);
            }
            await _db.SaveChangesAsync();

            return "Success!";
        }

        // Product
        public async Task<Product> CreateProduct(Product product)
        {
            if (product == default || string.IsNullOrEmpty(product.Name))
                return new Product();

            var _new = new Product() { Name = product.Name, Version = product.Version };
            await _db.Products!.AddAsync(_new);
            await _db.SaveChangesAsync();

            return _new;
        }
        public async Task<string> UpdateProduct(Product product)
        {
            var _product = await _db.Products!.FirstOrDefaultAsync(p => p.Name == product.Name);
            if (_product == default)
                return "Error!";

            _product.Name = product.Name;
            _product.Version = product.Version;
            _db.Products!.Update(_product);
            await _db.SaveChangesAsync();

            return "Success!";
        }
        public async Task<Product[]> GetProducts()
        {
            return await _db.Products!.ToArrayAsync();
        }
    }
}
