using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using OrderSystem.Controllers;
using OrderSystem.Data;
using OrderSystem.Models;
using OrderSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace OrderSystemTests
{
    public class CategoryControllerTests
    {

        private ProductDbContext _context;
        private CategoryController _categoryController;

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
            _categoryController = new CategoryController(dbContext: _context);
        }

        [TestCase(null, new string[] {"Books", "Electronics", "Tools"})]
        [TestCase("", new string[] { "Books", "Electronics", "Tools" })]
        public void Index_Returns_ListAscending_When_NotSpecified(string? input, string[] expected)
        {
            //Arrange
            _context.Categories.AddRange(
                new Category
                {
                    Id = 1,
                    Name = "Tools",
                    Products = new List<Product>
                    {
            new Product { Id = 1, Name = "Hammer" }
                    }
                },
                new Category
                {
                    Id = 2,
                    Name = "Books",
                    Products = new List<Product>
                    {
            new Product { Id = 2, Name = "Clean Code" },
            new Product { Id = 3, Name = "CLR via C#" }
                    }
                },
                new Category
                {
                    Id = 3,
                    Name = "Electronics",
                    Products = new List<Product>()
                }
            );
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            //Act
            var result = _categoryController.Index(input);

            //Assert

            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);

            var retrievedList = (viewResult!.Model as IEnumerable<object>)!.ToList();
            var retrievedNames = retrievedList
                        .Select(c => c.GetType().GetProperty("Name")!.GetValue(c) as string)
                        .ToList();

            Assert.That(retrievedNames, Is.EqualTo(expected));
        }


        [TestCase("name_desc", new string[] { "Tools", "Electronics", "Books" })]
        public void Index_Returns_ListDescending_When_Specified(string? input, string[] expected)
        {
            //Arrange
            _context.Categories.AddRange(
                new Category
                {
                    Id = 1,
                    Name = "Tools",
                    Products = new List<Product>
                    {
            new Product { Id = 1, Name = "Hammer" }
                    }
                },
                new Category
                {
                    Id = 2,
                    Name = "Books",
                    Products = new List<Product>
                    {
            new Product { Id = 2, Name = "Clean Code" },
            new Product { Id = 3, Name = "CLR via C#" }
                    }
                },
                new Category
                {
                    Id = 3,
                    Name = "Electronics",
                    Products = new List<Product>()
                }
            );
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            //Act
            var result = _categoryController.Index(input);

            //Assert

            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);

            var retrievedList = (viewResult!.Model as IEnumerable<object>)!.ToList();
            var retrievedNames = retrievedList
                        .Select(c => c.GetType().GetProperty("Name")!.GetValue(c) as string)
                        .ToList();

            Assert.That(retrievedNames, Is.EqualTo(expected));
        }


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _categoryController.Dispose();
        }
    }
}
