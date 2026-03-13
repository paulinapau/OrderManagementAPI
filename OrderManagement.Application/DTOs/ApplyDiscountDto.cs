using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OrderManagement.Application.DTOs
{
    public class ApplyDiscountDto
    {
        [Required]
        public string ProductName { get; set; } = null!;

        [Required]
        public decimal Percentage { get; set; } 

        [Required]
        public int MinQuantity { get; set; }              
    }
}
