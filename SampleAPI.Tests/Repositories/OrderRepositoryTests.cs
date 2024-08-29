using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SampleAPI.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private readonly DbContextOptions<SampleApiDbContext> _dbContextOptions;
        private readonly SampleApiDbContext _dbContext;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SampleApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new SampleApiDbContext(_dbContextOptions);
            _orderRepository = new OrderRepository(_dbContext);
        }

        [Fact]
        public void AddOrder_ShouldAddOrderAndReturnId()
        {
  
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Name = "Test Order",
                Description = "Test Description",
                EntryDate = DateTime.Now,
                IsInvoiced = true,
                IsDeleted = false
            };

  
            var resultId = _orderRepository.AddOrder(order);

  
            resultId.Should().Be(order.Id); 
            _dbContext.Orders.Should().ContainSingle(o => o.Id == order.Id); 
        }

        [Fact]
        public void GetRecentOrders_ShouldReturnOnlyRecentOrdersInDescendingOrder()
        {
  
            var recentOrder1 = new Order
            {
                Id = Guid.NewGuid(),
                Name = "Recent Order 1",
                Description = "Test Description 1",
                EntryDate = DateTime.Now,
                IsInvoiced = true,
                IsDeleted = false
            };

            var recentOrder2 = new Order
            {
                Id = Guid.NewGuid(),
                Name = "Recent Order 2",
                Description = "Test Description 2",
                EntryDate = DateTime.Now.AddHours(-1), 
                IsInvoiced = true,
                IsDeleted = false
            };

            var oldOrder = new Order
            {
                Id = Guid.NewGuid(),
                Name = "Old Order",
                Description = "Test Description 3",
                EntryDate = DateTime.Now.AddDays(-2),
                IsInvoiced = true,
                IsDeleted = false
            };

            var deletedOrder = new Order
            {
                Id = Guid.NewGuid(),
                Name = "Deleted Order",
                Description = "Test Description 4",
                EntryDate = DateTime.Now,
                IsInvoiced = true,
                IsDeleted = true 
            };

            _dbContext.Orders.AddRange(recentOrder1, recentOrder2, oldOrder, deletedOrder);
            _dbContext.SaveChanges();

  
            var recentOrders = _orderRepository.GetRecentOrders();

  
            recentOrders.Should().HaveCount(3); 
            recentOrders.First().Should().Be(recentOrder1); 
            recentOrders.Last().Should().Be(recentOrder2); 
            recentOrders.Should().NotContain(oldOrder); 
            recentOrders.Should().NotContain(deletedOrder); 
        }

        [Fact]
        public void GetOrdersWithinBusinessDays_ShouldReturnCorrectOrders()
        {
            var recentOrder1 = new Order
            {
                Id = Guid.NewGuid(),
                Name = "Order 1",
                Description = "Test Description 1",
                EntryDate = DateTime.Now.AddDays(-2), 
                IsInvoiced = true,
                IsDeleted = false
            };

            var recentOrder2 = new Order
            {
                Id = Guid.NewGuid(),
                Name = "Order 2",
                Description = "Test Description 2",
                EntryDate = DateTime.Now.AddDays(-4), 
                IsInvoiced = true,
                IsDeleted = false
            };

            var oldOrder = new Order
            {
                Id = Guid.NewGuid(),
                Name = "Old Order",
                Description = "Test Description",
                EntryDate = DateTime.Now.AddDays(-10),
                IsInvoiced = true,
                IsDeleted = false
            };

            _dbContext.Orders.AddRange(recentOrder1, recentOrder2, oldOrder);
            _dbContext.SaveChanges();

            var result = _orderRepository.GetOrdersWithinBusinessDays(3);
 
            result.Should().Contain(recentOrder1);
            result.Should().NotContain(oldOrder);
        }

    }
}
