using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Repositories;

namespace TaskManagementSystem.Tests;

public class UserRepositoryTests
{
    private DbContextOptions<TaskDbContext> _options;

    public UserRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TaskDbContext>()
                  .UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .Options;
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "John Doe", Email = "email@gmail.com", PasswordHash = "hash"};

        await using var context = new TaskDbContext(_options);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);
        var result = await repository.GetUserById(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("John Doe", result.Username);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        var user1 = new User { Id = Guid.NewGuid(), Username = "John Doe", Email = "email1@gmail.com", PasswordHash = "hash" };
        var user2 = new User { Id = Guid.NewGuid(), Username = "Jane Doe", Email = "email2@gmail.com", PasswordHash = "hash" };

        await using var context = new TaskDbContext(_options);
        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);
        var result = repository.GetUsers();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.Username == "John Doe");
        Assert.Contains(result, u => u.Username == "Jane Doe");
    }

    [Fact]
    public async Task CreateUser_ShouldAddUser()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "New User", Email = "email@gmail.com", PasswordHash = "hash" };

        await using var context = new TaskDbContext(_options);
        var repository = new UserRepository(context);
        await repository.InsertUser(user);
        await context.SaveChangesAsync();

        var result = await context.Users.FindAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("New User", result.Username);
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveUser()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "User to Delete", Email = "email@gmail.com", PasswordHash = "hash" };

        await using var context = new TaskDbContext(_options);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);
        await repository.DeleteUser(user);
        await context.SaveChangesAsync();

        var result = await context.Users.FindAsync(userId);

        Assert.Null(result);
    }
}
