using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using ShoppingKart.Models;

namespace ShoppingKart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public OrderController(AppDbContext context)
        {
            appDbContext = context;
        }

        // @type POST: api/Order
        // @desc add order
        [HttpPost]
        public async Task<ActionResult<Order>> CreatePrder(Order order)
        {
            bool isUserExist = appDbContext.Users.Any(dbUser => dbUser.Id == order.UserId);
            if (!isUserExist)
            {
                return NotFound(new { msg = $"User doesnot exist - invalid User id {order.UserId}" });
            }
            appDbContext.Orders.Add(order);
            try
            {
                await appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                bool isOrderExists = appDbContext.Orders.Any(dbOrder => dbOrder.Id == order.Id);
                if (isOrderExists)
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            CreatedAtAction("Orders", new { id = order.Id }, order);
            return Ok(new { msg = $"Order created", data = order });
       }

        // @type GET: api/Order
        // @desc get all orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await appDbContext.Orders.ToListAsync();
            return Ok(new { status = true, data = orders });
        }

        // @type GET: api/Order/1
        // @desc get order by id
        [HttpGet("{id}")]
        public ActionResult<Order> GetOrderById(int id)
        {
            var order = appDbContext.Orders
                .Where(order => order.Id == id)
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound(new { msg = $"Order with id: {id} not found"});
            }
            return Ok(new { status = true, order = order });
        }

        // @type DELETE api/Order/1
        // @desc delete order by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await appDbContext.Orders.FindAsync(id);
            if (order != null)
            {
                appDbContext.Orders.Remove(order);
                await appDbContext.SaveChangesAsync();
                var data = new { msg = $"Successfully Deleted", deleted_item = order };

                return Ok(data);
            }
            return NotFound(new { msg = $"Order with {id} not found" });
        }
    }
}
