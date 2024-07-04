using Models;
using System.Collections.Generic;

namespace WebApp.ViewModels
{
    public class CartViewModel
    {
        public Cart Cart { get; set; }
        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}