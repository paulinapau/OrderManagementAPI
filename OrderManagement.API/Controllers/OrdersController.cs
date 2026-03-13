using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;

namespace OrderManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(OrderService service) : ControllerBase
    {
        private readonly OrderService _service = service;

        /// <summary>
        /// Creates a new order with a list of products and their quantities.
        /// </summary>
        /// <param name="dto">Order data containing product names and quantities.</param>
        /// <returns>The created order.</returns>
        /// <response code="201">Order created successfully.</response>
        /// <response code="400">Request is invalid or order contains no products.</response>
        /// <response code="404">One or more products were not found.</response>
        [HttpPost("bulk")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(List<CreateOrderDto> orders)
        {
            if (orders == null || orders.Count == 0)
                return BadRequest("Orders list cannot be empty.");
            try
            {
                var createdOrders = await _service.CreateOrdersAsync(orders);
                return Created("/api/orders/bulk", createdOrders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }

        /// <summary>
        /// Retrieves a paginated list of orders from the database.
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of orders per page (default: 20)</param>
        /// <returns>Paginated list of orders</returns>
        /// <response code="200">Orders retrieved successfully</response>
        /// <response code="400">Invalid page or pageSize values</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int page = 1,[FromQuery] int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0 || pageSize > 100)
            {
                return BadRequest("Page or page size have incorect value.");
            }
            return Ok(await _service.GetAllOrdersAsync(page, pageSize));
        }

        /// <summary>
        /// Retrieves the invoice for a specific order by its order number.
        /// </summary>
        /// <param name="orderNumber">The unique number of the order.</param>
        /// <returns>The invoice details including products, quantities, discounts, and total amount.</returns>
        /// <response code="200">Invoice retrieved successfully.</response>
        /// <response code="404">No invoice found for the specified order number.</response>
        [HttpGet("{orderNumber}/invoice")]
        public async Task<IActionResult> GetInvoice(int orderNumber)
        {
            var invoice = await _service.GetInvoiceByNumberAsync(orderNumber);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }
    }
}