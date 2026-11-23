using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using OrderSystem.Controllers;
using OrderSystem.Data;
using OrderSystem.Models;
using OrderSystem.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;


namespace OrderSystemTests
{
    internal class ProductControllerTests
    {
        private ProductDbContext _context;

        private Mock<IProductService> _productServiceMock;

        private ProductController _productController;

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

            _productServiceMock = new Mock<IProductService>();

            _productController = new ProductController(_context, _productServiceMock.Object);
        }

        [Test]
        public void Index_Returns_ExpectedList_When_ValidInput()
        {
            //Arrange
            _productServiceMock.Setup(c => c.SearchProductsByName(It.IsAny<string>()))
                        .Returns(new List<Product> { new Product
                                {
                                    Id = 1,
                                    Name = "Gaming Laptop",
                                    Price = 999.99m,
                                    CategoryId = 1,
                                    Category = new Category { Id = 1, Name = "Electronics" }
                                } 
                        } as IEnumerable<Product>);
            var input = "Gaming";

            SeedProducts();

            var expected = new[] { "Gaming Laptop" };

            //Act
            var result = _productController.Index(input);

            //Assert

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);

            var model = viewResult.Model;
            var retrievedList = (model as IEnumerable<object>)!.ToList();

            var retrievedNames = retrievedList
                                    .Select(c => c.GetType().GetProperty("Name")!.GetValue(c) as string)
                                    .ToList();
            Assert.That(retrievedNames, Is.EqualTo(expected));
        }

        [Test]
        public void Index_Returns_AllProducts_When_SearchString_IsEmpty()
        {
            //Arrange
            SeedProducts();

            //Act
            var result = _productController.Index("");

            //Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);

            var model = viewResult!.Model as IEnumerable<Product>;
            Assert.That(model, Is.Not.Null);

            var retrievedNames = model!
                .Select(product => product.Name)
                .OrderBy(name => name)
                .ToList();

            Assert.That(retrievedNames, Is.EqualTo(new[] { "Gaming Laptop", "laptop stand", "Mechanical Keyboard" }.OrderBy(name => name).ToList()));
        }

        [Test]
        public void CreatePost_Adds_Product_When_ModelState_Valid()
        {
            //Arrange
            Product input = new Product
            {
                Id = 1,
                Name = "Gaming Laptop",
                Price = 999.99m,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Electronics" }
            };

            //Act
            var result = _productController.Create(input);

            //Assert

            var dbCount = _context.Products.Count();


            Assert.That(_productController.ModelState.IsValid, Is.True);

            Assert.That(dbCount, Is.EqualTo(1));
        }

        [Test]
        public void CreatePost_Redirects_To_Index()
        {
            //Arrange
            Product input = new Product
            {
                Id = 1,
                Name = "Gaming Laptop",
                Price = 999.99m,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Electronics" }
            };

            string expected = "Index";

            //Act
            var result = _productController.Create(input);

            //Assert

            var resultRedirect = result as RedirectToActionResult;

            Assert.That(resultRedirect, Is.Not.Null);

            Assert.That(resultRedirect.ActionName, Is.EqualTo(expected));
        }

        [Test]
        public void CreatePost_Reloads_When_ModelState_Invalid()
        {
            //Arrange
            Product? input = null;

            _productController.ModelState.AddModelError("default", "Product should be not null");


            //Act
            var result = _productController.Create(input);

            //Assert
            var resultView = result as ViewResult;


            Assert.That(resultView, Is.Not.Null);

            var model = resultView.Model  as Product;

            Assert.That(_productController.ModelState.IsValid, Is.False);

            Assert.That(model, Is.EqualTo(input));
        }

        [Test]
        public void CreateGet_Returns_ValidView()
        {
            //Arrange

            //Act
            var result = _productController.Create();

            //Assert
            var resultView = result as ViewResult;

            Assert.That(resultView, Is.Not.Null);
        }



        [Test]
        public void Details_Returns_ExpectedProduct_When_ValidId()
        {
            //Arrange
            SeedProducts();
            int input = 2;
            string expected = "laptop stand";

            //Act
            var result = _productController.Details(input);

            //Assert
            var resultView = result as ViewResult;

            Assert.That(resultView, Is.Not.Null);

            var model = resultView.Model as Product;

            var retrievedName = model!.Name;

            Assert.That(retrievedName, Is.EqualTo(expected));
        }

        [Test]
        public void Details_Returns_NotFound_When_InvalidId()
        {
            //Arrange
            SeedProducts();
            int input = 4;
            // The Details action does not use IProductService, so mocking is unnecessary

            //Act
            var result = _productController.Details(input);

            //Assert
            var resultView = result as NotFoundResult;

            Assert.That(resultView, Is.Not.Null);

        }


        private void SeedProducts()
        {
            var electronics = new Category { Id = 1, Name = "Electronics" };
            var accessories = new Category { Id = 2, Name = "Accessories" };

            _context.Categories.AddRange(electronics, accessories);
            _context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Gaming Laptop",
                    Price = 999.99m,
                    CategoryId = 1,
                    Category = electronics
                },
                new Product
                {
                    Id = 2,
                    Name = "laptop stand",
                    Price = 45.50m,
                    CategoryId = 2,
                    Category = accessories
                },
                new Product
                {
                    Id = 3,
                    Name = "Mechanical Keyboard",
                    Price = 129.99m,
                    CategoryId = 2,
                    Category = accessories
                }
            );

            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _productController.Dispose();
        }
    }
}
