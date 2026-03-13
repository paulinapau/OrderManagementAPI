using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ProductService service) : ControllerBase
{
    private readonly ProductService _service = service;

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

    /// <summary>
    /// Add multiple products to the database
    /// </summary>
    /// <param name="products">Product list with name and price</param>
    /// <returns>Created products</returns>
    /// <response code="201">Product list created successfully</response>
    /// <response code="409">Product with the same name already exists</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(List<ProductDto> products)
    {
        if (products == null || !products.Any())
        {
            return BadRequest("Product list cannot be empty.");
        }
        try
        {
            var productList = await _service.AddProductList(products);
            return Created("/bulk/", productList);
        }
        catch (DbUpdateException)
        {
            return Conflict("A product with this name already exists.");
        }
        catch (InvalidOperationException)
        {
            return Conflict("One or more products already exist in the database.");

        }
    }
    /// <summary>
    /// Search products by name with pagination
    /// </summary>
    /// <param name="name">Product name</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Created products</returns>
    /// <response code="201">Product list found successfully</response>
    /// <response code="400">Invalid page or pageSize values</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
    [FromQuery] string? name,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
    {
        if (page <= 0 || pageSize <= 0 || pageSize > 100)
        {
            return BadRequest("Page or page size have incorect value.");
        }
        var result = await _service.GetProducts(name, page, pageSize);

        return Ok(result);
    }
    /// <summary>
    /// Apply a discount to a product.
    /// </summary>
    /// <param name="dto">Product name, discount percentage, and minimum quantity required for the discount to apply.</param>
    /// <returns>The created discount.</returns>
    /// <response code="201">Discount created successfully.</response>
    /// <response code="404">Product was not found.</response>
    /// <response code="409">A discount with the same settings already exists.</response>
    /// <response code="400">Invalid request or unexpected error.</response>
    [HttpPost("discount")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApplyDiscount([FromBody] ApplyDiscountDto dto)
    {
        try
        {
            var discount = await _service.ApplyDiscountAsync(dto);
            return CreatedAtAction(nameof(GetDiscount), new { id = discount.Id }, discount);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Unexpected error occurred.");
        }
    }

    /// <summary>
    /// Retrieves a report for a discounted product.
    /// </summary>
    /// <param name="productName">The name of the product to generate the discount report for.</param>
    /// <returns>A report containing the product name, discount (%), number of orders including this discounted product, 
    /// and total amount ($) ordered for this product.</returns>
    /// <response code="200">Discounted product report retrieved successfully.</response>
    /// <response code="404">No orders found for the specified discounted product.</response>
    [HttpGet("discount-report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDiscountReport([FromQuery] string productName)
    {
        var report = await _service.GetDiscountedProductReportAsync(productName);

        if (report == null)
            return NotFound($"No orders found for discounted product '{productName}'.");

        return Ok(report);
    }

    [HttpGet("discount/{id}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetDiscount(Guid id)
    {
        var discount = await _service.GetDiscountByIdAsync(id);
        if (discount == null)
            return NotFound("No discount");
        return Ok(discount);
    }
}