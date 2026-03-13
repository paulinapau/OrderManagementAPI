using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task<bool> ProductExistsAsync(List<string> names);
        Task AddRangeAsync(List<Product> products);
        Task<(List<Product> Items, int TotalCount)> GetProductsAsync(string? name, int page, int pageSize);
        Task<Product?> GetProductByNameAsync(string name);
        Task<bool> DiscountExistsAsync(Guid productId, decimal minQuantity, decimal Percentage);
        Task<Discount> AddDiscountAsync(Discount discount);
        Task<Discount?> GetDiscountByIdAsync(Guid id);
        Task<decimal> GetActiveDiscountAsync(Guid productId, decimal quantity);
        Task<List<DiscountedProductReportDto>> GetDiscountedProductReportAsync(string productName);
    }
}
