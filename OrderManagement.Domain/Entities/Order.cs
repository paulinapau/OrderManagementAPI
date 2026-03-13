using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Number { get; set; } = 0;
        public decimal TotalAmount => Products.Sum(op => op.Product.Price * op.Quantity);
        public ICollection<OrderProduct> Products { get; set; } = []; 
    }
}
