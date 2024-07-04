using System;
using System.Collections.Generic;
using models;

namespace Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}