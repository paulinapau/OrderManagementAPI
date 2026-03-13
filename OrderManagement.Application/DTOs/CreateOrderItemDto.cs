using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs
{
    public class CreateOrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
}
