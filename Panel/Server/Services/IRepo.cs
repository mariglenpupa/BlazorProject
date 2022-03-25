using Panel.Shared.Classes;
using Panel.Shared.Interfaces;

namespace Panel.Server.Services
{
    public interface IRepo
    {
        // User
        public Task<string> ManageUser(IUser user, string IP, bool register);
        public Task<User> GetUser(string userToken);
        public Task<User[]> GetUsers();
        public Task<string> UpdateUser(IUser user);

        // Key
        public Task<Key> CreateKey(IKey key);
        public Task<string> ClaimKey(IKey key);
        public Task<string> CheckKeys(IKey key);
        public Task<Key[]> GetKeys();
        public Task<string> UpdateKey(IKey key);

        // Product
        public Task<Product> CreateProduct(Product product);
        public Task<string> UpdateProduct(Product product);
        public Task<Product[]> GetProducts();
    }
}
