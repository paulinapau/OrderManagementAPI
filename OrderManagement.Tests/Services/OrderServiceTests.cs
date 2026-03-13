using Xunit;
using Moq;
using FluentAssertions;
using OrderManagement.Application.Services;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OrderManagement.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldThrow_WhenNoProducts()
        {
            var dto = new CreateOrderDto { Products = new List<CreateOrderItemDto>() };

            Func<Task> act = () => _orderService.CreateOrderAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Order must have products*");
        }
        [Fact]
        public async Task CreateOrderAsync_ShouldThrow_WhenQuantityZeroOrNegative()
        {
            var dto = new CreateOrderDto
            {
                Products = new List<CreateOrderItemDto>
                {
                    new() { ProductName = "Apple", Quantity = 0 }
                }
            };

            Func<Task> act = () => _orderService.CreateOrderAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Quantity must be greater than zero*");
        }
        [Fact]
        public async Task CreateOrderAsync_ShouldThrow_WhenProductNotFound()
        {
            var dto = new CreateOrderDto
            {
                Products = new List<CreateOrderItemDto>
                {
                    new() { ProductName = "Apple", Quantity = 2 }
                }
            };

            _productRepositoryMock
                .Setup(r => r.GetProductByNameAsync("Apple"))
                .ReturnsAsync((Product)null);

            Func<Task> act = () => _orderService.CreateOrderAsync(dto);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Apple*not found*");
        }
        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrderSuccessfully()
        {
            var productId = Guid.NewGuid();
            var dto = new CreateOrderDto
            {
                Products = new List<CreateOrderItemDto>
                {
                    new() { ProductName = "Apple", Quantity = 2 }
                }
            };

            var product = new Product { Id = productId, Name = "Apple", Price = 10m };

            _productRepositoryMock
                .Setup(r => r.GetProductByNameAsync("Apple"))
                .ReturnsAsync(product);

            _productRepositoryMock
                .Setup(r => r.GetActiveDiscountAsync(productId, 2))
                .ReturnsAsync(10m);

            _orderRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            var result = await _orderService.CreateOrderAsync(dto);

            result.Products.Should().HaveCount(1);
            result.Products.First().ProductId.Should().Be(productId);
            result.Products.First().Quantity.Should().Be(2);
            result.Products.First().Discount.Should().Be(10m);
            result.TotalAmount.Should().Be(2 * 10 * 0.9m);

            _orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
        }
        [Fact]
        public async Task GetInvoiceByNumberAsync_ShouldThrow_WhenOrderNotFound()
        {
            _orderRepositoryMock.Setup(r => r.GetByNumberAsync(123))
                .ReturnsAsync((Order)null);

            Func<Task> act = () => _orderService.GetInvoiceByNumberAsync(123);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*123*not found*");
        }
        [Fact]
        public async Task GetInvoiceByNumberAsync_ShouldReturnInvoice()
        {
            var productId = Guid.NewGuid();
            var order = new Order
            {
                Products = new List<OrderProduct>
                {
                    new OrderProduct
                    {
                        Product = new Product { Id = productId, Name = "Apple", Price = 10m },
                        Quantity = 2,
                        Discount = 10m,
                        Price = 10m
                    }
                }
            };

            _orderRepositoryMock.Setup(r => r.GetByNumberAsync(123))
                .ReturnsAsync(order);

            var result = await _orderService.GetInvoiceByNumberAsync(123);

            result.OrderNumber.Should().Be(123);
            result.Products.Should().HaveCount(1);
            var item = result.Products[0];
            item.Name.Should().Be("Apple");
            item.Amount.Should().Be(2 * 10 * 0.9m);
            result.TotalAmount.Should().Be(2 * 10 * 0.9m);
        }
    }

}
