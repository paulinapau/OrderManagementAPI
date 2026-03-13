using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Domain.Entities
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
       
     
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

     
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
