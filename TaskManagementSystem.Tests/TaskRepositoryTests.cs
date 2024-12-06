using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Repositories;

namespace TaskManagementSystem.Tests;

public class TaskRepositoryTests
{
    private readonly DbContextOptions<TaskDbContext> _options;

    public TaskRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnTask()
    {
        var taskId = Guid.NewGuid();
        var userTask = new UserTask { Id = taskId, Title = "Title"};

        await using var context = new TaskDbContext(_options);
        context.Tasks.Add(userTask);
        await context.SaveChangesAsync();

        var repository = new TaskRepository(context);
        var result = await repository.GetTaskById(taskId);

        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
    }

    [Fact]
    public async Task AddTask_ShouldAddTask()
    {
        var userTask = new UserTask { Id = Guid.NewGuid(), Title = "Title" };

        await using var context = new TaskDbContext(_options);
        var repository = new TaskRepository(context);
        await repository.InsertTask(userTask);
        await context.SaveChangesAsync();

        var result = await context.Tasks.FindAsync(userTask.Id);
        Assert.NotNull(result);
        Assert.Equal(userTask.Id, result.Id);
    }

    [Fact]
    public async Task UpdateTask_ShouldUpdateTask()
    {
        var userTask = new UserTask { Id = Guid.NewGuid(), Title = "Title" };

        await using var context = new TaskDbContext(_options);
        context.Tasks.Add(userTask);
        await context.SaveChangesAsync();

        userTask.Title = "Updated Task";
        var repository = new TaskRepository(context);
        await repository.UpdateTask(userTask);
        await context.SaveChangesAsync();

        var result = await context.Tasks.FindAsync(userTask.Id);
        Assert.NotNull(result);
        Assert.Equal("Updated Task", result.Title);
    }

    [Fact]
    public async Task DeleteTask_ShouldDeleteTask()
    {
        var userTask = new UserTask { Id = Guid.NewGuid(), Title = "Title" };

        await using var context = new TaskDbContext(_options);
        context.Tasks.Add(userTask);
        await context.SaveChangesAsync();

        var repository = new TaskRepository(context);
        await repository.DeleteTask(userTask);
        await context.SaveChangesAsync();

        var result = await context.Tasks.FindAsync(userTask.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllTasks_ShouldReturnAllTasks()
    {
        var userTask1 = new UserTask { Id = Guid.NewGuid(), Title = "Title1" };
        var userTask2 = new UserTask { Id = Guid.NewGuid(), Title = "Title2" };

        await using var context = new TaskDbContext(_options);
        context.Tasks.AddRange(userTask1, userTask2);
        await context.SaveChangesAsync();

        var repository = new TaskRepository(context);
        var result = repository.GetTasks().Count();

        Assert.Equal(2, result);
    }
}