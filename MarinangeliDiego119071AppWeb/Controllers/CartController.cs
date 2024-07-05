using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;
using WebApp.ViewModels;
using Models;

public class CartController : Controller
{
    private readonly CartService _cartService;
    private readonly ProductService _productService;

    public CartController(CartService cartService, ProductService productService)
    {
        _cartService = cartService;
        _productService = productService;
    }

    // GET: /Cart/AddToCartForm
    public async Task<IActionResult> AddToCartForm(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        return View(new AddToCartViewModel { ProductId = productId });
    }

    // POST: /Cart/AddToCart
    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("AddToCartForm", model);
        }

        try
        {
            // Supponiamo che l'ID utente venga ottenuto dal contesto di autenticazione, qui hardcoded per semplicità
            var userId = "test-user-id";

            // Recupera o crea un carrello per l'utente
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartService.CreateCartAsync(cart);
            }

            // Aggiungi il prodotto al carrello
            await _cartService.AddToCartAsync(cart.Id, model.ProductId, model.Quantity);

            // Reindirizza alla vista del carrello
            return RedirectToAction("ViewCart");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Failed to add product to cart. Please try again later.");
            return View("AddToCartForm", model);
        }
    }
}