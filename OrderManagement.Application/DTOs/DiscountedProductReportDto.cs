using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs
{
    public class DiscountedProductReportDto
    {
        public string ProductName { get; set; } = null!;
        public decimal Discount { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
