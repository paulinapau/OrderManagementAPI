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
        Task<bool> ProductExistsAsync(string name);
        Task AddRangeAsync(List<Product> products);
        Task<(List<Product> Items, int TotalCount)> GetProducts(string? name, int page, int pageSize);
        Task<Product?> GetProductByNameAsync(string name);
        Task<bool> DiscountExistsAsync(Guid productId, int minQuantity, decimal Percentage);
        Task<Discount> AddDiscountAsync(Discount discount);
        Task<Discount?> GetDiscountByIdAsync(Guid id);
    }
}
