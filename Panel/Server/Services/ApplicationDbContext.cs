using Microsoft.EntityFrameworkCore;
using Panel.Shared.Classes;

namespace Panel.Server.Services
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("DataSource=Main.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => { entity.HasIndex(e => e.UserName).IsUnique(); });
            modelBuilder.Entity<User>(entity => { entity.HasIndex(e => e.UUID).IsUnique(); });
            modelBuilder.Entity<Key>().HasOne(k => k.User).WithMany(u => u.ActiveKeys);
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<Key>? Keys { get; set; }
    }
}
