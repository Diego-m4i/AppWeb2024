using System;
using System.Collections.Generic;

namespace Models
{
    public class Cart
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}