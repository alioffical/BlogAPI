using BlogAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly BlogContext _context;
        

        public BlogPostsController(BlogContext context)
        {
            _context = context;
        }
        [Route("BlogPosts")]
        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.Title) || string.IsNullOrEmpty(blogPost.Author))
            {
                return BadRequest("Title and Author are required.");
            }

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return Ok("Data has been added successfully");
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetPost(int id)
        {
            try
            {
                var post = await _context.BlogPosts.FindAsync(id);

                if (post == null)
                {
                    return NotFound(new { Message = "Post not found." });
                }

                return Ok(post);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { Message = "An error occurred while accessing the database.", Details = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpGet("GetAllPosts")]
        public async Task<ActionResult<IEnumerable<object>>> GetPosts()
        {
            try
            {
                var posts = await _context.BlogPosts
                    .Select(p => new
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Author = p.Author,
                        Quote = p.Content.Length > 100 ? p.Content.Substring(0, 100) : p.Content
                    })
                    .ToListAsync();

                return Ok(posts);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { Message = "An error occurred while accessing the database.", Details = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

    }
}
