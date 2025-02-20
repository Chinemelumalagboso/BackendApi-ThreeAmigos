using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductApi.Controllers;
using ProductApi.Data;
using ProductApi.Dtos;
using ProductApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApi.Test
{
    [TestClass]
    public class ProductsControllerTests
    {
        private ProductsController _controller;
        private ApplicationDbContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Use InMemory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureDeleted(); // Clear previous data
            _context.Database.EnsureCreated();

            _context.Products.AddRange(new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 10, Description = "Test Product", CategoryId = 1 },
                new Product { Id = 2, Name = "Product2", Price = 20, Description = "Another Product", CategoryId = 2 }
            });

            _context.SaveChanges();

            _controller = new ProductsController(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose(); // Dispose database after test
        }

        [TestMethod]
        public async Task GetProducts_ReturnsOk_WithProductList()
        {
            var result = await _controller.GetProducts();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var products = okResult.Value as IEnumerable<ProductDto>;
            Assert.IsNotNull(products);
            Assert.AreEqual(2, products.Count());
        }

        
    }
}
