using BlogAPI.Controllers;
using BlogAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlogAPI.Tests
{
    public class BlogPostsControllerTests
    {
        private readonly BlogPostsController _controller;
        private readonly BlogContext _context;

        public BlogPostsControllerTests()
        {
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "blogdb")
                .Options;

            _context = new BlogContext(options);
            _controller = new BlogPostsController(_context);
        }

        [Fact]
        public async Task PostBlogPost_ValidBlogPost_ReturnsOkResult()
        {
            
            var blogPost = new BlogPost { Title = "Test Title", Author = "Test Author", Content = "This is a test blog post.",Quote="" };

          
            var result = await _controller.PostBlogPost(blogPost);

           
            var okResultTemp = Assert.IsType<ActionResult<BlogPost>>(result);
            var okResult = okResultTemp.Result as OkObjectResult;

            Assert.Equal("Data has been added successfully", okResult.Value);
            Assert.Equal(1, await _context.BlogPosts.CountAsync());
        }

        [Fact]
        public async Task PostBlogPost_InvalidBlogPost_ReturnsBadRequest()
        {
            var blogPost = new BlogPost { Title = "", Author = "Test Author", Content = "This is a test blog post." };

            var result = await _controller.PostBlogPost(blogPost);

            var actionResult = Assert.IsType<ActionResult<BlogPost>>(result);
            var badRequestResult = actionResult.Result as BadRequestObjectResult; 

            Assert.NotNull(badRequestResult);
            Assert.Equal("Title and Author are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetPost_ExistingId_ReturnsBlogPost()
        {
            var blogPost = new BlogPost
            {
                Title = "Test Title",
                Author = "Test Author",
                Content = "This is a test blog post.",
                Quote="",
                Id=0
            };

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            var result = await _controller.GetPost(blogPost.Id);

            var actionResult = Assert.IsType<ActionResult<BlogPost>>(result);
            var returnedPost = Assert.IsType<BlogPost>(actionResult.Value);

            Assert.Equal(blogPost.Title, returnedPost.Title);
            Assert.Equal(blogPost.Author, returnedPost.Author);
            Assert.Equal(blogPost.Content, returnedPost.Content);
        }

        [Fact]
        public async Task GetPost_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.GetPost(999); 

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task GetPosts_ReturnsAllPosts()
        {
            var post1 = new BlogPost
            {
                Title = "Test Title",
                Author = "Test Author",
                Content = "This is a test blog post.",
                Quote = "",
                Id = 1
            }; var post2 = new BlogPost
            {
                Title = "Test ",
                Author = "Test ",
                Content = "This is a  blog post.",
                Quote = "5",
                Id =2
            };

            _context.BlogPosts.Add(post1);
            _context.BlogPosts.Add(post2);
            await _context.SaveChangesAsync();

            var result = await _controller.GetPosts();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<object>>>(result);
            var okResult = actionResult.Result as OkObjectResult; 

            Assert.NotNull(okResult); 
            var posts = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            var postList = posts.ToList();

            Assert.Contains(postList, p =>
        p.GetType().GetProperty("Title")?.GetValue(p)?.ToString() == post1.Title &&
        p.GetType().GetProperty("Author")?.GetValue(p)?.ToString() == post1.Author
    );

            Assert.Contains(postList, p =>
                p.GetType().GetProperty("Title")?.GetValue(p)?.ToString() == post2.Title &&
                p.GetType().GetProperty("Author")?.GetValue(p)?.ToString() == post2.Author
            );

        }
    }
    
}
