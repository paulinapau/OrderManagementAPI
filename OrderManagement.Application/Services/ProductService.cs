using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using System.Xml.Linq;

namespace OrderManagement.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Product>> GetAllProducts() => await _repository.GetAllAsync();
    public async Task<List<Product>> AddProductList(List<ProductDto> productDtos)
    { 
        var names = productDtos.Select(p => p.Name).ToList();

        if (names.Count != names.Distinct().Count())
            throw new InvalidOperationException("The request contains duplicate product names.");

        if (await _repository.ProductExistsAsync(names))
            throw new InvalidOperationException("One or more products already exist in the database.");


        var products = productDtos.Select(dto => new Product
        {
            Name = dto.Name,
            Price = dto.Price
        }).ToList();

        await _repository.AddRangeAsync(products);
        return products;
    }

    public async Task<object> GetProducts(string? name, int page, int pageSize)
    {
        var result = await _repository.GetProductsAsync(name, page, pageSize);

        var products = result.Items.Select(p => new ProductDto
        {
            Name = p.Name,
            Price = p.Price
        });

        return new
        {
            page,
            pageSize,
            totalItems = result.TotalCount,
            items = products
        };
    }
    
    public async Task<Discount> ApplyDiscountAsync(ApplyDiscountDto dto)
    {
        var product = await _repository.GetProductByNameAsync(dto.ProductName);

        if (product == null)
            throw new KeyNotFoundException($"Product '{dto.ProductName}' not found.");

        bool exists = await _repository.DiscountExistsAsync(product.Id, dto.MinQuantity, dto.Percentage);
        if (exists)
            throw new InvalidOperationException(
                $"A discount for '{dto.ProductName}' with min quantity {dto.MinQuantity} and percentage {dto.Percentage} already exists."
            );
        if (dto.Percentage < 0 || dto.Percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(dto.Percentage), "Discount must be between 0 and 100");

        var discount = new Discount
        {
            ProductId = product.Id,
            Percentage = dto.Percentage,
            MinQuantity = dto.MinQuantity
        };

        return await _repository.AddDiscountAsync(discount);
    }
    public async Task<Discount?> GetDiscountByIdAsync(Guid id)
    {
        return await _repository.GetDiscountByIdAsync(id);
    }
    public async Task<List<DiscountedProductReportDto>?> GetDiscountedProductReportAsync(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.", nameof(productName));

        return await _repository.GetDiscountedProductReportAsync(productName);
    }
}

