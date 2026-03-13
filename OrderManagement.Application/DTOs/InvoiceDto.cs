using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OrderManagement.Application.DTOs
{
    public class InvoiceDto
    {
        public int OrderNumber { get; set; }  
        public List<InvoiceProductDto> Products { get; set; } = new();
        public decimal TotalAmount { get; set; } 
    }

    public class InvoiceProductDto
    {
        public required string Name { get; set; } 
        public decimal Quantity { get; set; }  
        public decimal Price { get; set; } 
        public decimal Discount { get; set; }
        public decimal Amount { get; set; } 
    }
}
