using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository,
                            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }
        public async Task<List<Order>> CreateOrdersAsync(List<CreateOrderDto> ordersDto)
        {
            var orders = new List<Order>();

            foreach (var dto in ordersDto)
            {
                var order = await CreateOrderAsync(dto);
                orders.Add(order);
            }

            return orders;
        }
        public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
        {
            var order = new Order();
            decimal totalAmount = 0;

            if (dto.Products.Count == 0)
                throw new ArgumentException("Order must have products.");

            foreach (var item in dto.Products)
            {
                if (item.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than zero.");

                var product = await _productRepository.GetProductByNameAsync(item.ProductName);

                if (product == null)
                    throw new KeyNotFoundException($"Product '{item.ProductName}' not found.");

                var existing = order.Products.FirstOrDefault(op => op.ProductId == product.Id);

                if (existing != null)
                {
                    totalAmount -= existing.Price * existing.Quantity * (1 - (existing.Discount / 100m));

                    existing.Quantity += item.Quantity;

                    var discount = await _productRepository.GetActiveDiscountAsync(product.Id, existing.Quantity);
                    existing.Discount = discount;

                    totalAmount += existing.Price * existing.Quantity * (1 - (discount / 100m));
                }
                else
                {
                    var discount = await _productRepository.GetActiveDiscountAsync(product.Id, item.Quantity);

                    var orderProduct = new OrderProduct
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price,
                        Discount = discount
                    };

                    order.Products.Add(orderProduct);

                    totalAmount += orderProduct.Price * orderProduct.Quantity * (1 - (discount / 100m));
                }
            }
            order.TotalAmount = totalAmount;

            await _orderRepository.CreateAsync(order);

            return order;
        }
        public async Task<object> GetAllOrdersAsync(int page, int pageSize)
        {
            var result = await _orderRepository.GetAllAsync(page, pageSize);

            return new
            {
                page,
                pageSize,
                totalOrders = result.totalCount,
                result.orders,
            };
        }
        public async Task<InvoiceDto> GetInvoiceByNumberAsync(int ordernumber)
        {

            var order = await _orderRepository.GetByNumberAsync(ordernumber);

            if (order == null)
                throw new KeyNotFoundException($"Order with number '{ordernumber}' not found.");

            var products = order.Products.Select(op => new InvoiceProductDto
            {
                Name = op.Product.Name,
                Quantity = op.Quantity,
                Discount = op.Discount,
                Price = op.Price,
                Amount = (op.Price * op.Quantity) * (1 - (op.Discount / 100m))
            }).ToList();
            var invoice = new InvoiceDto
            {
                OrderNumber = ordernumber,
                Products = products,
                TotalAmount = products.Sum(p => p.Amount)
            };
            return invoice;

        }
    }

}
