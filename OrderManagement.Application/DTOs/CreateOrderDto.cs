using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs
{
    public class CreateOrderDto
    {
        public List<CreateOrderItemDto> Products { get; set; } = new();
    }
}
