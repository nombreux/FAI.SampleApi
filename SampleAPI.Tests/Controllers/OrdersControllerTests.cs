using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SampleAPI.Controllers;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SampleAPI.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ILogger<OrdersController>> _mockLogger;
        private readonly OrdersController _ordersController;

        public OrdersControllerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _ordersController = new OrdersController(_mockOrderRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetRecentOrders_ShouldReturnOkResultWithRecentOrders()
        {
 
            var mockOrders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), Name = "Order 1", Description = "Test Order 1", EntryDate = DateTime.Now },
                new Order { Id = Guid.NewGuid(), Name = "Order 2", Description = "Test Order 2", EntryDate = DateTime.Now.AddHours(-1) }
            };

            _mockOrderRepository.Setup(repo => repo.GetRecentOrders()).Returns(mockOrders);

  
            var result = await _ordersController.GetRecentOrders();

  
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(mockOrders);
        }

        [Fact]
        public async Task GetRecentOrders_ShouldReturnInternalServerErrorOnException()
        {
 
            _mockOrderRepository.Setup(repo => repo.GetRecentOrders()).Throws(new Exception("Test exception"));

  
            var result = await _ordersController.GetRecentOrders();

  
            var objectResult = result.Result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("An error occurred while retrieving orders.");
           // _mockLogger.Verify(logger => logger.LogError(It.IsAny<Exception>(), "An error occurred while getting recent orders."), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnCreatedAtActionResultWhenOrderIsValid()
        {
 
            var request = new CreateOrderRequest { Name = "Order 1", Description = "Test Order 1", IsInvoiced = true };
            var newOrder = new Order { Id = Guid.NewGuid(), Name = request.Name, Description = request.Description, IsInvoiced = request.IsInvoiced };

            _mockOrderRepository.Setup(repo => repo.AddOrder(It.IsAny<Order>())).Returns(newOrder.Id);

  
            var result = await _ordersController.CreateOrder(request);

  
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            createdAtActionResult.Should().NotBeNull();
            createdAtActionResult.StatusCode.Should().Be(201);
            createdAtActionResult.Value.Should().BeEquivalentTo(request);
           // _mockLogger.Verify(logger => logger.LogInformation("Order created successfully with ID {Id}.", newOrder.Id), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnBadRequestWhenRequestIsInvalid()
        {
 
            var request = new CreateOrderRequest { Name = "", Description = "" }; 

  
            var result = await _ordersController.CreateOrder(request);

  
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("Order name and description are required.");
        }


        [Fact]
        public async Task CreateOrder_ShouldReturnInternalServerErrorOnException()
        {
 
            var request = new CreateOrderRequest { Name = "Order 1", Description = "Test Order 1", IsInvoiced = true };
            _mockOrderRepository.Setup(repo => repo.AddOrder(It.IsAny<Order>())).Throws(new Exception("Test exception"));

  
            var result = await _ordersController.CreateOrder(request);

  
            var objectResult = result.Result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("An error occurred while creating the order.");
        }

        [Fact]
        public async Task GetOrdersWithinBusinessDays_ShouldReturnOkResultWithOrders()
        {
            // Arrange
            var mockOrders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), Name = "Order 1", Description = "Test Order 1", EntryDate = DateTime.Now.AddDays(-2) },
                new Order { Id = Guid.NewGuid(), Name = "Order 2", Description = "Test Order 2", EntryDate = DateTime.Now.AddDays(-4) }
            };

            _mockOrderRepository.Setup(repo => repo.GetOrdersWithinBusinessDays(It.IsAny<int>())).Returns(mockOrders);

            // Act
            var result = await _ordersController.GetOrdersWithinBusinessDays(3);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(mockOrders);
        }

    }
}
