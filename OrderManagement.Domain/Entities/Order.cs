using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OrderManagement.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Number { get; set; } 
        public decimal TotalAmount { get; set; }
        public ICollection<OrderProduct> Products { get; set; } = []; 
    }
}
