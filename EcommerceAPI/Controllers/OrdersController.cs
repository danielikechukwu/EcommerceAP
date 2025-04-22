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
    public class OrdersController : ControllerBase
    {

        private readonly EcommerceDbContext _context;

        public OrdersController(EcommerceDbContext context)
        {
            _context = context;
        }

        // Create a new order.
        // Demonstrates [FromBody]
        // Endpoint: POST /api/orders/CreateOrder
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderDTO orderDto)
        {
            // Validate Customer existence
            var customer = _context.Customer.FindAsync(orderDto.CustomerId);

            if(customer == null)
            {
                return BadRequest("Customer does not exist");
            }

            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Processing",
                OrderAmount = 0, //Will calculate based on OrderItems
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            // Iterate through order items and add to order
            foreach (var item in orderDto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null)
                {
                    return BadRequest($"Product with ID {item.ProductId} does not exist");
                }

                if(product.Stock < item.Quantity)
                {
                    return BadRequest($"Insufficient stock for product {product.Name}");
                }

                //Deduct stock
                product.Stock -= item.Quantity;

                totalAmount += item.Quantity * product.Price;

                //Create order item
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                };

                order.OrderItems.Add(orderItem);
            }

            order.OrderAmount = totalAmount;

            _context.Order.Add(order);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id}, order);

        }

        // Get an order by ID.
        // Demonstrates [FromRoute] 
        // Endpoint: GET /api/orders/GetOrderById/{id}
        [HttpGet("GetOrderById/{id}")]
        public async Task<ActionResult<Order>> GetOrderById([FromRoute] int id)
        {
            var order = await _context.Order
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if(order == null)
            {
                return NotFound($"The searched Order with ID {id}, could not be found");
            }

            return Ok(order);
        }
    }
}
