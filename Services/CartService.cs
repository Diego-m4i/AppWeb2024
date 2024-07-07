using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using WebApp.data;

namespace Services
{
    public class CartService
    {
        private readonly AppDb _dbContext;
        private readonly IDataProtector _protector;

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
            /*var encryptedCartId = _protector.Protect(cartId); // Cripta il cartId*/

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
        public void UpdateCartItemQuantity(int cartItemId, int quantity)
        {
            var cartItem = _dbContext.CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _dbContext.SaveChanges();
            }
        }
        public async Task<decimal> CalculateCartTotalAsync(string cartId)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return 0;
            }

            decimal total = cart.CartItems.Sum(ci => ci.Quantity * ci.Price);
            return total;
        }

        public async Task ClearCartAsync(string cartId)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart != null)
            {
                _dbContext.Carts.Remove(cart);
                await _dbContext.SaveChangesAsync();
            }
        }
    
    }
}
