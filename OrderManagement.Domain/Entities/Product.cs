using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public ICollection<Discount> Discounts { get; set; } = [];
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
}