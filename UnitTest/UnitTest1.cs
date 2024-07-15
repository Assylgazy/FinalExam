using Microsoft.EntityFrameworkCore;
using Reddit;
using Reddit.Repositories;
using Reddit.Models;

namespace UnitTest;

public class UnitTest1
{
    private IPostsRepository GetPostsRepostory()
    {

        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: dbName).Options;


        var dbContext = new ApplicationDbContext(options);

        dbContext.Posts.Add(new Post { Title = "Title 1", Content = "Content 1", Upvotes = 10, Downvotes = 16 });
        dbContext.Posts.Add(new Post { Title = "Title 2", Content = "Content 1", Upvotes = 11, Downvotes = 17 });
        dbContext.Posts.Add(new Post { Title = "Title 3", Content = "Content 1", Upvotes = 12, Downvotes = 18 });
        dbContext.Posts.Add(new Post { Title = "Title 4", Content = "Content 1", Upvotes = 13, Downvotes = 19 });
        dbContext.Posts.Add(new Post { Title = "Title 5", Content = "Content 1", Upvotes = 14, Downvotes = 20 });
        dbContext.SaveChanges();


        return new PostsRepository(dbContext);
    }

    [Fact]
    public async Task GetPosts_ReturnsCorrectPagination()
    {
        var postsRepository = GetPostsRepostory();
        var posts = await postsRepository.GetPosts(1, 3, null, null, null);


        Assert.Equal(3, posts.Items.Count);
        Assert.Equal(5, posts.TotalCount);
        Assert.True(posts.HasNextPage);
        Assert.False(posts.HasPreviousPage);
    }


    [Fact]
    public async Task GetPosts_ReturnsCorrect()
    {

        var postsRepository = GetPostsRepostory();
        var posts = await postsRepository.GetPosts(1, 3, null, "popular", false);

        Assert.Equal(3, posts.Items.Count);
        Assert.Equal(5, posts.TotalCount);
        Assert.True(posts.HasNextPage);
        Assert.False(posts.HasPreviousPage);


        Assert.Equal("Title 5", posts.Items.First().Title);
    }


    [Fact]
    public async Task GetPosts_InvalidPageSize_ThrowsArgumentOutOfRangeException()
    {
        var repository = GetPostsRepostory();



        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(page: 1, pageSize: 0, searchTerm: null, SortTerm: null));
        Assert.Equal("pageSize", exception.ParamName);
    }

    [Fact]
    public async Task GetPosts_InvalidPage_ThrowsArgumentException()
    {

        var repository = GetPostsRepostory();



        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(page: 0, pageSize: 10, searchTerm: null, SortTerm: null));
        Assert.Equal("page", exception.ParamName);
    }
}

