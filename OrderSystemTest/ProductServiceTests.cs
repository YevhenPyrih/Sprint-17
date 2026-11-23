using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using OrderSystem.Data;
using OrderSystem.Models;
using OrderSystem.Services;
using System;
using System.Collections.Generic;
using System.Text;


namespace OrderSystemTests
{
    internal class ProductServiceTests
    {
        private ProductDbContext _context;

        private ProductService _productService;

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

            _productService = new ProductService(_context);
        }

        [Test]
        public void SearchProductsByName_Returns_CorrectList_When_ValidSubstring()
        {
            //Arrange
            SeedProducts();
            string input = "Keyboard";
            var expected = new[] { "Mechanical Keyboard" };

            //Act

            var result = _productService.SearchProductsByName(input);

            //Assert
            Assert.That(result, Is.Not.Null);

            var retrievedNames = result.Select(c => c.Name).ToList();

            Assert.That(retrievedNames, Is.EqualTo(expected));

        }

        [Test]
        public void SearchProductsByName_Returns_AllProducts_When_Substring_IsWhitespace()
        {
            //Arrange
            SeedProducts();

            //Act
            var result = _productService.SearchProductsByName("   ");

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Select(product => product.Name).OrderBy(name => name),
                Is.EqualTo(new[] { "Gaming Laptop", "laptop stand", "Mechanical Keyboard" }.OrderBy(name => name)));
        }

        [TestCase(1, new string[] { "Gaming Laptop" })]
        [TestCase(2, new string[] { "laptop stand", "Mechanical Keyboard" })]
        public void GetProductsByCategory_Returns_CorrectList(int input, string[] expected)
        {
            //Arrange
            SeedProducts();

            //Act 
            var result = _productService.GetProductsByCategory(input);

            //Assert
            Assert.That(result, Is.Not.Null);

            var retrievedNames = result.Select(c => c.Name).OrderBy(c => c).ToList();

            Assert.That(retrievedNames, Is.EqualTo(expected));


        }
        [TestCase("Price", new string[] { "laptop stand", "Mechanical Keyboard", "Gaming Laptop" })]
        [TestCase("CategoryId", new string[] { "Gaming Laptop", "laptop stand", "Mechanical Keyboard" })]
        public void GetProductsSorted_Returns_Sorted(string input, string[] expected)
        {
            SeedProducts();

            //Act 
            var result = _productService.GetProductsSorted(input);

            //Assert
            Assert.That(result, Is.Not.Null);

            var retrievedNames = result.Select(c => c.Name).ToList();

            Assert.That(retrievedNames, Is.EqualTo(expected));
        }


        [Test]
        public void SearchProducts_Returns_AllProducts_When_SearchTerm_IsEmpty()
        {
            //Arrange
            SeedProducts();

            //Act
            var result = _productService.SearchProducts("");

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Select(product => product.Name).OrderBy(name => name),
                Is.EqualTo(new[] { "Gaming Laptop", "laptop stand", "Mechanical Keyboard" }.OrderBy(name => name)));
        }

        [Test]
        public void SearchProducts_Returns_CorrectList_When_ValidSubstring()
        {
            //Arrange
            SeedProducts();
            string input = "stand";
            var expected = new[] { "laptop stand" };

            //Act

            var result = _productService.SearchProducts(input);

            //Assert
            Assert.That(result, Is.Not.Null);

            var retrievedNames = result.Select(c => c.Name).ToList();

            Assert.That(retrievedNames, Is.EqualTo(expected));

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
        }
    }
}
