using FluentAssertions;
using Moq;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _service = new ProductService(_repositoryMock.Object);
        }

        [Fact]
        public async Task AddProductList_ShouldThrow_WhenDuplicateNamesInRequest()
        {
            var products = new List<ProductDto>
            {
                new() { Name = "Apple", Price = 1 },
                new() { Name = "Apple", Price = 2 }
            };

            Func<Task> act = async () => await _service.AddProductList(products);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*duplicate*");
        }
        [Fact]
        public async Task AddProductList_ShouldThrow_WhenProductAlreadyExists()
        {
            var products = new List<ProductDto>
            {
                new() { Name = "Apple", Price = 1 },
                new() { Name = "Banana", Price = 2 }
            };

            _repositoryMock
                .Setup(r => r.ProductExistsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(true);

            Func<Task> act = () => _service.AddProductList(products);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already exist*");
        }
        [Fact]
        public async Task AddProductList_ShouldCreateProducts_WhenTheyDoNotExist()
        {
            var products = new List<ProductDto>
            {
                new() { Name = "Apple", Price = 1 },
                new() { Name = "Banana", Price = 2 }
            };

            _repositoryMock
                .Setup(r => r.ProductExistsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(false);

            var result = await _service.AddProductList(products);

            result.Should().HaveCount(2);
            result.Select(p => p.Name).Should().Contain(new[] { "Apple", "Banana" });

            _repositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<List<Product>>()), Times.Once);
        }
        [Fact]
        public async Task ApplyDiscountAsync_ShouldThrow_WhenProductNotFound()
        {
            var dto = new ApplyDiscountDto
            {
                ProductName = "Apple",
                MinQuantity = 5,
                Percentage = 10
            };

            _repositoryMock
                .Setup(r => r.GetProductByNameAsync(dto.ProductName))
                .ReturnsAsync((Product)null);

            Func<Task> act = () => _service.ApplyDiscountAsync(dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("*Apple*not found*");
        }
        [Fact]
        public async Task ApplyDiscountAsync_ShouldThrow_WhenDiscountAlreadyExists()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Apple" };

            var dto = new ApplyDiscountDto
            {
                ProductName = "Apple",
                MinQuantity = 5,
                Percentage = 10
            };

            _repositoryMock
                .Setup(r => r.GetProductByNameAsync(dto.ProductName))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(r => r.DiscountExistsAsync(product.Id, dto.MinQuantity, dto.Percentage))
                .ReturnsAsync(true);

            Func<Task> act = () => _service.ApplyDiscountAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*already exists*");
        }
        [Fact]
        public async Task ApplyDiscountAsync_ShouldThrow_WhenPercentageOutOfRange()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Apple" };

            var dto = new ApplyDiscountDto
            {
                ProductName = "Apple",
                MinQuantity = 5,
                Percentage = 120
            };

            _repositoryMock
                .Setup(r => r.GetProductByNameAsync(dto.ProductName))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(r => r.DiscountExistsAsync(product.Id, dto.MinQuantity, dto.Percentage))
                .ReturnsAsync(false);

            Func<Task> act = () => _service.ApplyDiscountAsync(dto);

            await act.Should()
                .ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*Discount must be between 0 and 100*");
        }
        [Fact]
        public async Task ApplyDiscountAsync_ShouldCreateDiscount_WhenValid()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Apple" };

            var dto = new ApplyDiscountDto
            {
                ProductName = "Apple",
                MinQuantity = 5,
                Percentage = 10
            };

            var createdDiscount = new Discount
            {
                ProductId = productId,
                MinQuantity = 5,
                Percentage = 10
            };

            _repositoryMock
                .Setup(r => r.GetProductByNameAsync(dto.ProductName))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(r => r.DiscountExistsAsync(product.Id, dto.MinQuantity, dto.Percentage))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(r => r.AddDiscountAsync(It.IsAny<Discount>()))
                .ReturnsAsync(createdDiscount);

            var result = await _service.ApplyDiscountAsync(dto);

            result.ProductId.Should().Be(productId);
            result.MinQuantity.Should().Be(5);
            result.Percentage.Should().Be(10);

            _repositoryMock.Verify(r =>
                r.AddDiscountAsync(It.IsAny<Discount>()), Times.Once);
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetDiscountedProductReportAsync_ShouldThrow_WhenProductNameIsInvalid(string invalidName)
        {
            Func<Task> act = () => _service.GetDiscountedProductReportAsync(invalidName);

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("*Product name is required*")
                .Where(e => e.ParamName == "productName");
        }
        [Fact]
        public async Task GetDiscountedProductReportAsync_ShouldReturnReport_WhenRepositoryReturnsList()
        {
            var productName = "Apple";
            var report = new List<DiscountedProductReportDto>
            {
                new()
                {
                    ProductName = "Apple",
                    Discount = 10m,
                    OrdersCount = 5,
                    TotalAmount = 50m
                }
    }       ;

            _repositoryMock
                .Setup(r => r.GetDiscountedProductReportAsync(productName))
                .ReturnsAsync(report);

            var result = await _service.GetDiscountedProductReportAsync(productName);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);

            var item = result![0];
            item.ProductName.Should().Be("Apple");
            item.Discount.Should().Be(10m);
            item.OrdersCount.Should().Be(5);
            item.TotalAmount.Should().Be(50m);
        }
        [Fact]
        public async Task GetDiscountedProductReportAsync_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            var productName = "Banana";

            _repositoryMock
                .Setup(r => r.GetDiscountedProductReportAsync(productName))
                .ReturnsAsync((List<DiscountedProductReportDto>?)null);

            var result = await _service.GetDiscountedProductReportAsync(productName);

            result.Should().BeNull();
        }
    }
}
