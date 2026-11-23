using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using OrderSystem.Controllers;
using OrderSystem.Data;
using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace OrderSystemTests
{
    internal class OrderControllerTests
    {
        private ProductDbContext _context;

        private OrderController _orderController;

        [SetUp]
        public void Setup()
        {
            DbContextOptions<ProductDbContext> options = new DbContextOptionsBuilder<ProductDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid
                            .NewGuid()
                            .ToString()
                            )
                    .Options;
            _context = new ProductDbContext(options);

            _orderController = new OrderController(_context);
        }

        [Test]
        public void CreateGet_Returns_View_With_DropdownProductList()
        {
            //Arrange
            Type expected = typeof(SelectList);
            //Act
            var result = _orderController.Create();

            //Assert
            var resultView = result as ViewResult;

            var typeOfList = _orderController.ViewBag.Products.GetType();

            Assert.That(resultView, Is.Not.Null);

            Assert.That(typeOfList, Is.EqualTo(expected));
        }


        [Test]
        public void CreatePost_Adds_Order_When_ModelState_Valid()
        {
            //Arrange
            Order input = new Order
            {
                Id = 1,
                UserId = "1",
                User = new User { Id = 1, Name = "alice" },
                OrderDate = new DateTime(2026, 5, 10),
                Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                Id = 1,
                                OrderId = 1,
                                ProductId = 1,
                                Quantity = 2,
                                Product = new Product
                                {
                                    Id = 1,
                                    Name = "Gaming Laptop",
                                    Price = 999.99m,
                                    CategoryId = 1,
                                    Category = new Category { Id = 1, Name = "Electronics" }
                                }
                            }
                        }
            };

            //Act
            var result = _orderController.Create(input);

            //Assert

            var dbCount = _context.Orders.Count();


            Assert.That(_orderController.ModelState.IsValid, Is.True);

            Assert.That(dbCount, Is.EqualTo(1));
        }



        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _orderController.Dispose();
        }
    }
}
