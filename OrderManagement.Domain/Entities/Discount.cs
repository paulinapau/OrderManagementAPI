using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace OrderManagement.Domain.Entities
{
    public class Discount
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal MinQuantity { get; set; } = 0;
        public decimal Percentage { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; } = null!;
    }
}
