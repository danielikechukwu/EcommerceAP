using EcommerceAPI.Data;
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
    }
}
