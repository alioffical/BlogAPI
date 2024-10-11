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

        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            if (string.IsNullOrEmpty(blogPost.Title) || string.IsNullOrEmpty(blogPost.Author))
            {
                return BadRequest("Title and Author are required.");
            }

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = blogPost.Id }, blogPost);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetPost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPosts()
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
    }
}
