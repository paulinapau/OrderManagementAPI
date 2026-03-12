using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
