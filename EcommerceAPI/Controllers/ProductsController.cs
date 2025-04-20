using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly EcommerceDbContext _context;

        public ProductsController(EcommerceDbContext context)
        {
            _context = context;
        }

        // Get all products with optional filtering by name, category, and price range.
        // Demonstrates [FromQuery] and default binding.
        // Endpoint: GET /api/products/GetProducts?name={name}&category={category}&minPrice={minPrice}&maxPrice={maxPrice}
        [HttpGet("GetProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct(
            [FromQuery] string? name,
            [FromQuery] string? category,
            [FromQuery] decimal? minPrice, 
            [FromQuery] decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            if(!string.IsNullOrEmpty(name) )
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Name.Contains(category));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= maxPrice.Value);
            }

            var products = await query.ToListAsync();

            return Ok(products);
        }

        // Get a specific product by ID.
        // Demonstrates [FromRoute] and default binding.
        // Endpoint: GET /api/products/GetProductById/{id}
        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<Product>> GetProductById([FromRoute] string id)
        {
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound("Product could not be found");
            }

            return Ok(product);
        }

        // Create a new product.
        // Demonstrates [FromBody] binding.
        // Endpoint: POST /api/products/CreateProduct
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductCreateDTO productCreateDTO)
        {
            // Mapping from ProductCreateDto to Product entity
            var product = new Product
            {
                Name = productCreateDTO.Name,
                Category = productCreateDTO.Category,
                Price = productCreateDTO.Price,
                Description = productCreateDTO.Description,
                Stock = productCreateDTO.Stock,
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            //Created with location header
            return CreatedAtAction(nameof(GetProductById), new {id = product.Id}, product);

        }

        // Update an existing product's price.
        // Demonstrates multiple binding attributes.
        // Endpoint: PUT /api/products/UpdateProductPrice/{id}?price={newPrice}
        [HttpPut("UpdateProductPrice/{id}")]
        public async Task<IActionResult> UpdateProductPrice([FromRoute] string id, [FromQuery] decimal price)
        {
            var product = await _context.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound("Product in question could be found");
            }

            product.Price = price;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Demonstrates [FromQuery] for pagination
        // Endpoint: GET: /api/products/paged?pageNumber={pageNumber}&pageSize={pageSize}
        [HttpGet("paged")]
        public async Task<ActionResult<IList<Product>>> GetProductPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {

            var product = await _context.Products.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            return Ok(product);

        }

        // Upload product image.
        // Demonstrates [FromForm]
        // Endpoint: POST /api/products/{id}/upload
        [HttpPost("{id}/upload")]
        public async Task<ActionResult> UploadProductImage([FromRoute] int id, [FromForm] IFormFile file)
        {
            if(file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var product = await _context.Products.FindAsync(id);

            if(product == null)
                return NotFound("Product in question does not exist");

            // For demonstration, we'll just read the file name.
            // In a real application, you'd save the file to storage and update the product's image URL.

            var fileName = Path.GetFileName(file.FileName);
            // TODO: Save the file and update product.ImageUrl

            return Ok(new { Message = "Image uploaded successfully", FileName = fileName });

        }
    }
}
