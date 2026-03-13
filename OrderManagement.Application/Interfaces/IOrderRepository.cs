using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task CreateAsync(Order order);
        Task<(List<Order> orders, int totalCount)> GetAllAsync(int page, int pageSize);
        Task<Order?> GetByNumberAsync(int orderNumber);
    }
}
