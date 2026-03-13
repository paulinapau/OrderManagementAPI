using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Infrastructure.Persistence
{
    public class OrderRepository: IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        public async Task<(List<Order> orders, int totalCount)> GetAllAsync(int page, int pageSize)
        {
            pageSize = Math.Min(pageSize, 100);
            var query = _context.Orders
                .Include(o => o.Products)
                .ThenInclude(op => op.Product) 
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(o => o.Number)   
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Order?> GetByNumberAsync(int orderNumber)
        {
            return await _context.Orders
                .Include(o => o.Products)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Number == orderNumber);
        }
    }
}
