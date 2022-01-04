using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingKart.Models;


namespace ShoppingKart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public CartController(AppDbContext context)
        {       
            appDbContext = context;
        }

        // @type POST: api/Cart
        // @desc add product to cart
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {

            bool isProductExists = appDbContext.Products.Any(dbProduct => dbProduct.Id == cart.ProductId);
            if (!isProductExists)
            {
                Response.StatusCode = 500;
                
                return Content("{\"msg\": \"Product doesnot exist - invalid Product id\"}");
            }
            appDbContext.Carts.Add(cart);

            try
            {
                await appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                bool isCartExists = appDbContext.Carts.Any(dbCart => dbCart.Id == cart.Id);
                if (isCartExists)
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

             CreatedAtAction("Carts", new { id = cart.Id }, cart);
            var data = new { msg = $"Product added to cart ", cart = cart };

            return Ok(data);

        }

        // @type GET: api/Cart/5
        // @desc get cart by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await appDbContext.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }
            var cartItem = await appDbContext.Products.FindAsync(cart.ProductId);
            var data = new { status = true, data = new { id = cart.Id, product = cartItem, userId = cart.UserId, quantity = cart.Quantity } };

            return Ok(data);
        }

        // @type PUT: api/Cart/5
        // @desc update cart by id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (id != cart.Id)
            {
                return BadRequest();
            }
            bool isProductExists = appDbContext.Products.Any(dbProduct => dbProduct.Id == cart.ProductId);
            if (!isProductExists)
            {
                Response.StatusCode = 500;

                return Content("{\"msg\": \"Product doesnot exist - invalid Product id\"}");
            }
            appDbContext.Entry(cart).State = EntityState.Modified;

            try
            {
                await appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool isCartExists = appDbContext.Carts.Any(dbCart => dbCart.Id == id);
                if (!isCartExists)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var data = new { msg = "Cart Updated", data = cart };

            return Ok(data);
        }
    }
}
