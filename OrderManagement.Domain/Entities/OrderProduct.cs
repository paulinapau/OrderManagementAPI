using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OrderManagement.Domain.Entities
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }  
        public decimal Discount{ get; set; }

        public Guid OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; } = null!;

        public Guid ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; } = null!;
    }
}
