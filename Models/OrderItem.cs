using Models;

namespace models;

using System;

    public class OrderItem
    {
        public Guid OrderItemId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }

