﻿using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly EcommerceDbContext _context;

        public CustomersController(EcommerceDbContext context)
        {
            _context = context;
        }

        //Register new customer
        //Demostrate FromForm
        //Endpoint: POST /api/customers/register
        [HttpPost("register")]
        public async Task<ActionResult<Customer>> RegisterCustomer([FromForm] CustomerRegistrationDTO registerCustomer)
        {

            try
            {
                //Check if email already exist
                bool exists = await _context.Customer.AnyAsync(c => c.Email == registerCustomer.Email);

                if (exists)
                {
                    return BadRequest("Customer with email already exist");
                }

                //Create new customer
                var customer = new Customer
                {
                    Name = registerCustomer.Name,
                    Email = registerCustomer.Email,
                    Password = registerCustomer.Password
                };

                _context.Customer.Add(customer);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Login a customer.
        // Demonstrates [FromHeader] and [FromBody].
        // Endpoint: POST /api/customers/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromHeader(Name = "X-Client-ID")] string clientId, [FromBody] CustomerLoginDTO loginDTO)
        {

            try
            {
                //Check the header
                if (string.IsNullOrWhiteSpace(clientId))
                    return BadRequest("Missing X-Client-ID header, Client Unauthorized");

                var customer = await _context.Customer.FirstOrDefaultAsync(c => c.Email == loginDTO.Email && c.Password == loginDTO.Password);

                if (customer == null)
                {
                    return Unauthorized("Invalid email and password");
                }

                //Generate JWT or other token in real application
                return Ok(new { Message = "Authentication successful." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get customer details.
        // Demonstrates default binding (from route or query).
        // Endpoint: GET /api/customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer([FromRoute] int id)
        {

            try
            {
                var customer = await _context.Customer.FindAsync(id);

                if (customer == null)
                {
                    return NotFound("Sorry result could not be found");
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
