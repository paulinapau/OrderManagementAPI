using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Application.DTOs;

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
}
