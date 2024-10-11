using Xunit;
using Moq;
using BlogAPI.Controllers;
using BlogAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BlogAPI;
using BlogAPI.DTOs;

public class BlogPostsControllerTests
{
    private readonly BlogPostsController _controller;
    private readonly Mock<BlogContext> _mockContext;

    public BlogPostsControllerTests()
    {
        _mockContext = new Mock<BlogContext>();

      
        var mockSet = new Mock<DbSet<BlogPost>>();

        
        var blogPosts = new List<BlogPost>
        {
            new BlogPost { Id = 1, Title = "Test Post 1", Author = "Author 1", Content = "Content 1" },
            new BlogPost { Id = 2, Title = "Test Post 2", Author = "Author 2", Content = "Content 2" }
        }.AsQueryable();

        mockSet.As<IQueryable<BlogPost>>().Setup(m => m.Provider).Returns(blogPosts.Provider);
        mockSet.As<IQueryable<BlogPost>>().Setup(m => m.Expression).Returns(blogPosts.Expression);
        mockSet.As<IQueryable<BlogPost>>().Setup(m => m.ElementType).Returns(blogPosts.ElementType);
        mockSet.As<IQueryable<BlogPost>>().Setup(m => m.GetEnumerator()).Returns(blogPosts.GetEnumerator());

       
        _mockContext.Setup(c => c.BlogPosts).Returns(mockSet.Object);

       
        _controller = new BlogPostsController(_mockContext.Object);
    }

    [Fact]
    public async Task GetPosts_ReturnsAllBlogPosts()
    {
        var result = await _controller.GetPosts();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<BlogPostDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<BlogPostDto>>(okResult.Value);

        Assert.Equal(2, returnValue.Count());
    }
}
