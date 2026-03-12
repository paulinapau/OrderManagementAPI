using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Application.DTOs;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ProductService service) : ControllerBase
{
    private readonly ProductService _service = service;

    //[HttpGet]
    //public async Task<IActionResult> Get() => Ok(await _service.GetAllProducts());

    /// <summary>
    /// Retrieves all products from the database
    /// </summary>
    /// <returns>List of all products</returns>
    /// <response code="200">List of products retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetAllProducts());
    }
    [HttpPost]

    /// <summary>
    /// Add a single product to the database
    /// </summary>
    /// <param name="request">Product with name and price</param>
    /// <returns>Created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="409">Product with the same name already exists</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(ProductDto productdto)
    {
        if (await _service.ProductExists(productdto.Name))
        {
            return Conflict($"A product with the name '{productdto.Name}' already exists.");
        }

        var product = await _service.AddProduct(productdto);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }
}