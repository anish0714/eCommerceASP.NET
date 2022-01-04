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
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public CommentController(AppDbContext context)
        {
            appDbContext = context;
        }
        // @type POST: api/Comment
        // @desc add a comment to product
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            bool isProductExists = appDbContext.Products.Any(dbProduct => dbProduct.Id == comment.ProductId);
            if (!isProductExists)
            {
                return NotFound(new { msg = "Product doesnot exist - invalid Product id" });
            }
            appDbContext.Comments.Add(comment);
            try
            {
                await appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                bool isCommentExists = appDbContext.Comments.Any(dbComment => dbComment.Id == comment.Id);
                if (isCommentExists)
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

             CreatedAtAction("Comments", new { id = comment.Id }, comment);
            var data = new { msg = $"Comment added to product", data = comment };
            return Ok(data);
        }

        // @type GET: api/Comment/1
        // @desc get commment by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await appDbContext.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound(new { msg = $"Comment with {id} does not exist"});
            }
            var product = await appDbContext.Products.FindAsync(comment.ProductId);
            var data = new { status = true, data = new { id = comment.Id, product = product, userId = comment.UserId, text = comment.Text,
                rating = comment.Rating, imageUrl = comment.Image
            } };

            return Ok(data);
        }

        // @type DELETE: api/Comment/1
        // @desc delete comment by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await appDbContext.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            appDbContext.Comments.Remove(comment);
            await appDbContext.SaveChangesAsync();
            var data = new { msg = $"Successfully Deleted", deleted_item = comment };
            return Ok(data);
        }

        // @type PUT: api/Comment/5
        // @desc update comment by id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
           
            if (id != comment.Id)
            {
                return BadRequest();
            }
            bool isProductExists = appDbContext.Products.Any(dbProduct => dbProduct.Id == comment.ProductId);
            if (!isProductExists)
            {
                return NotFound(new { msg = "Product doesnot exist - invalid Product id" });
            }
            appDbContext.Entry(comment).State = EntityState.Modified;

            try
            {
                await appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool isCommentExists = appDbContext.Comments.Any(dbComment => dbComment.Id == comment.Id);

                if (!isCommentExists)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var data = new { msg = "Comment Updated", data = comment };

            return Ok(data);
        }
    }
}
