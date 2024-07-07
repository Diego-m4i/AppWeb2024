using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace WebApp.data
{
    public class AppDb : IdentityDbContext<ApplicationUser>
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        // Non è necessario definire DbSet per ApplicationUser, viene gestito da IdentityDbContext
    }
}