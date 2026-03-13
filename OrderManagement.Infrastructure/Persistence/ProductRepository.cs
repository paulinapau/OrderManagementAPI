using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync() => await _context.Products.Include(d => d.Discounts).ToListAsync();
    public async Task<Product?> GetByIdAsync(Guid id) => await _context.Products.FindAsync(id);
    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> ProductExistsAsync(string name)
    {
        return await _context.Products
            .AnyAsync(p => p.Name == name);
    }
    public async Task AddRangeAsync(List<Product> products)
    {
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
    }
    public async Task<(List<Product> Items, int TotalCount)> GetProducts(string? name, int page, int pageSize)
    {
        pageSize = Math.Min(pageSize, 100);
        var query = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product?> GetProductByNameAsync(string name)
    {
        return await _context.Products
            .Include(p => p.Discounts)
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<bool> DiscountExistsAsync(Guid productId, int minQuantity, decimal percentage)
    {
        return await _context.Discounts
            .AnyAsync(d => d.ProductId == productId && d.MinQuantity == minQuantity && d.Percentage == percentage);
    }

    public async Task<Discount> AddDiscountAsync(Discount discount)
    {
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
        return discount;
    }
    public async Task<Discount?> GetDiscountByIdAsync(Guid id)
    {
        return await _context.Discounts.FindAsync(id);
    }
}
