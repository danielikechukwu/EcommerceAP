using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public CustomerController(EcommerceDbContext context)
        {
            _context = context;
        }

        //Register new customer
        //Demostrate FromForm
        //Endpoint: POST /api/customers/register
        //[HttpPost("register")]
        //public async Task<ActionResult<Customer>> RegisterCustomer([FromForm] CustomerRegistrationDTO registerCustomer)
        //{

        //}

    }
}
