using System;
using System.Collections.Generic;

namespace Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public string Status { get; set; }
    }
}