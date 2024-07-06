using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApp.data;

namespace Services
{
    public class CartService
    {
        private readonly AppDb _dbContext;

        public CartService(AppDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == "Active");
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId) ?? new Cart { UserId = userId, Status = "Active" };
            
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                var product = await _dbContext.Products.FindAsync(productId);
                if (product != null)
                {
                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.Price
                    });
                }
            }

            _dbContext.Carts.Update(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int cartItemId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var cartItem = cart?.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);
                if (!cart.CartItems.Any())
                {
                    _dbContext.Carts.Remove(cart);
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
