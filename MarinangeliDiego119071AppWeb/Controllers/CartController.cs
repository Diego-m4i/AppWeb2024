using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;
using Models;
using System;
using WebApp.ViewModels;

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

        return View(new CartViewModel() { Product = product });
    }

    // POST: /Cart/AddToCart
    [HttpPost]
    public async Task<IActionResult> AddToCart(CartViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("AddToCartForm", model);
        }

        try
        {
            // Logica per aggiungere il prodotto al carrello

            return RedirectToAction("ViewCart");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Failed to add product to cart. Please try again later.");
            return View("AddToCartForm", model);
        }
    }



}