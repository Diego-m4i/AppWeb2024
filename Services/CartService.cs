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

        public async Task<Cart> GetCartByIdAsync(string cartId)
        {
            return await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId && c.Status == "Open");
        }

        public async Task AddToCartAsync(string cartId, int productId, int quantity)
        {
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = cartId,
                    Status = "Open"
                };
                _dbContext.Carts.Add(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = (await _dbContext.Products.FindAsync(productId)).Price
                };
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string cartId, int cartItemId)
        {
            var cart = await GetCartByIdAsync(cartId);
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
