using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using ShoppingKart.Models;


namespace ShoppingKart
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController :  ControllerBase
    {
        private readonly AppDbContext appDbContext;
        public ProductController(AppDbContext context)
        {
            appDbContext = context;
        }

        // @type GET: api/Product
        // @desc get all products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await appDbContext.Products.ToListAsync();
            var data = new { status = true, data = products };
            return Ok(data);
            
        }


        // @type GET: api/Product/1
        // @desc get product by id
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = appDbContext.Products
                .Where(product => product.Id == id)
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }
            var data = new { status = true, product = product};
            return Ok(data);
        }


        // @type POST: api/Product
        // @desc add a product
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            appDbContext.Products.Add(product);
            try
            {
                await appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                bool isProductExist = appDbContext.Products.Any(e => e.Id == product.Id);
                if (isProductExist)
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            CreatedAtAction("Products", new { id = product.Id }, product);
            var data = new { msg = $"Product added Successfully ", product = product };

            return Ok(data);
        }

        // @type DELETE api/Product/1
        // @desc delete product by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await appDbContext.Products.FindAsync(id);
            if (product != null)
            {
                appDbContext.Products.Remove(product);
                await appDbContext.SaveChangesAsync();
                var data = new { msg = $"Successfully Deleted", deleted_item = product };

                return Ok(data);
            }
            return NotFound(new { msg = $"Product with {id} not found"});
        }

    }
}
