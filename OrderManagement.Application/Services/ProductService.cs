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
    public async Task<Product> AddProduct(ProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price
        };
        await _repository.AddAsync(product);
        return product;
    }
    public async Task<bool> ProductExists(string name)
    {
        return await _repository.ProductExistsAsync(name.ToLower());
    } 
    public async Task<List<Product>> AddProductList(List<ProductDto> productDtos)
    {
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
        var result = await _repository.GetProducts(name, page, pageSize);

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

        var discount = new Discount
        {
            ProductId = product.Id,
            Percentage = dto.Percentage,
            MinQuantity = dto.MinQuantity
        };

        return await _repository.AddDiscountAsync(discount);
    }
    public async Task<Discount> GetDiscountByIdAsync(Guid id)
    {
        return await _repository.GetDiscountByIdAsync(id);
    }
}

