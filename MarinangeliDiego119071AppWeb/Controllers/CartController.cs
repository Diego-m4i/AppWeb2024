using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.ViewModels;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddToCartAsync(userId, productId, quantity);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.RemoveFromCartAsync(userId, cartItemId);
            return RedirectToAction("ViewCart");
        }
    }
}