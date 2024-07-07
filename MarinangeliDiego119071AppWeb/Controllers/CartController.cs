using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> ViewCart()
        {
            var cartId = HttpContext.Session.GetString("CartId");
            var cart = await _cartService.GetCartByIdAsync(cartId);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var cartId = HttpContext.Session.GetString("CartId");
            if (cartId == null)
            {
                cartId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartId", cartId);
            }

            await _cartService.AddToCartAsync(cartId, productId, quantity);
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartId = HttpContext.Session.GetString("CartId");
            await _cartService.RemoveFromCartAsync(cartId, cartItemId);
            return RedirectToAction("ViewCart");
        }
    
        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity, string operation)
        {
            if (operation == "increment")
            {
                quantity++;
            }
            else if (operation == "decrement" && quantity > 1)
            {
                quantity--;
            }

            _cartService.UpdateCartItemQuantity(cartItemId, quantity);
            return RedirectToAction(nameof(ViewCart));
        }
        public async Task<IActionResult> Checkout()
        {
            var cartId = "get_your_cart_id_here"; // Sostituisci con il metodo corretto per ottenere l'ID del carrello
            var cart = await _cartService.GetCartByIdAsync(cartId);

            if (cart == null || !cart.CartItems.Any())
            {
                return View("EmptyCart");
            }

            return View(cart);
        }
        
        [HttpPost]
        public async Task<IActionResult> Checkout(Cart cart)
        {

            return View("CheckoutConfirmation", cart);
        }
        
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string cardNumber, string expiryDate, string cvv)
        {
            return View("CheckoutConfirmation");
        }
        
    }
}