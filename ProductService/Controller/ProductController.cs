using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Data;

namespace ProductService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly DbServiceLayer _dbService;

        public ProductController(ILogger<ProductController> logger, DbServiceLayer dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            _logger.LogInformation("Fetching all products");
            // Fetch products from the database
            var products = _dbService.GetProductsAsync().GetAwaiter().GetResult();
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            _logger.LogInformation("Creating a new product: {ProductName}", product.Name);
            // Add product to the database
            var result = await _dbService.AddProductAsync(product);
            if (result <= 0)
            {
                _logger.LogError("Failed to create product: {ProductName}", product.Name);
                return BadRequest("Failed to create product");
            }
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            _logger.LogInformation("Updating product {ProductId}: {ProductName}", id, product.Name);
            // Update product in the database
            var existingProduct = _dbService.GetProductsAsync().GetAwaiter().GetResult().FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", id);
                return NotFound();
            }
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            var result = _dbService.UpdateProductAsync(existingProduct).GetAwaiter().GetResult();
            if (result <= 0)
            {
                _logger.LogError("Failed to update product {ProductId}: {ProductName}", id, product.Name);
                return BadRequest("Failed to update product");
            }
            return Content("Product updated successfully");
        }
    }
}
