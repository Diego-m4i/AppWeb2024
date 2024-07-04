using Microsoft.EntityFrameworkCore;
using WebApp.data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task CreateCartAsync(Cart cart)
        {
            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddToCartAsync(Guid cartId, int productId, int quantity)
        {
            var cartItem = await _dbContext.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _dbContext.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}