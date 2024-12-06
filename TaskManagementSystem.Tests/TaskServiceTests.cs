using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using TaskManagementSystem.BLL.Errors;
using TaskManagementSystem.BLL.Services;
using TaskManagementSystem.BLL.Validators;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly TaskService _taskService;

    
    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        IValidator<CreateUserTaskDto> taskValidatorMock = new CreateUserTaskDtoValidator();
        IValidator<TaskFilter> filterValidator = new TaskFilterValidator();
        IValidator<TaskSort> sortValidator = new TaskSortValidator();
        IValidator<TaskPagination> paginationValidator = new TaskPaginationValidator();
        Mock<ILogger<TaskService>> loggerMock = new();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RegisterDto, User>();
            cfg.CreateMap<UserTaskDto, UserTask>();
            cfg.CreateMap<CreateUserTaskDto, UserTask>();
            cfg.CreateMap<UserTask, UserTaskDto>();
            cfg.CreateMap<User, UserDto>();
        }); 
        var mapper = config.CreateMapper();
        _taskService = new TaskService(
            _taskRepositoryMock.Object,
            loggerMock.Object,
            taskValidatorMock,
            mapper,
            _userRepositoryMock.Object,
            filterValidator,
            sortValidator,
            paginationValidator
        );
    }

    [Fact]
    public async Task CreateUserTask_ValidationFails_ReturnsTaskCreationFailed()
    {
        var userId = Guid.NewGuid();
        var userTaskDto = new CreateUserTaskDto { Title = "", Status = 0, Priority = 0};

        var result = await _taskService.CreateUserTask(userId, userTaskDto);
        
        var stringResult = result.Match<string>(s => s.Id.ToString(), f => f.ErrorMessage);
        
        Assert.Equal("'Title' must not be empty.", stringResult);
    }

    [Fact]
    public async Task CreateUserTask_UserNotFound_ReturnsTaskCreationFailed()
    {
        var userId = Guid.NewGuid();
        var userTaskDto = new CreateUserTaskDto { Title = "Title", Priority = 0, Status = 0};
        _userRepositoryMock.Setup(u => u.GetUsers()).Returns(new List<User>().AsQueryable().BuildMock());

        var result = await _taskService.CreateUserTask(userId, userTaskDto);
        
        var stringResult = result.Match<string>(s => s.Id.ToString(), f => f.ErrorMessage);

        Assert.Equal("User doesn't exist", stringResult);
    }

    [Fact]
    public async Task CreateUserTask_Success_ReturnsUserTaskDto()
    {
        var userId = Guid.NewGuid();
        var userTaskDto = new CreateUserTaskDto { Title = "Test Task", Priority = 0, Status = 0 };
        var user = new User { Id = userId };
        var taskId = Guid.NewGuid();
        _userRepositoryMock.Setup(u => u.GetUsers()).Returns(new List<User> { user }.AsQueryable().BuildMock());
        _taskRepositoryMock.Setup(t => t.InsertTask(It.IsAny<UserTask>())).Returns(Task.FromResult(new UserTask
        {
            Id = taskId,
            Title = userTaskDto.Title,
            Priority = userTaskDto.Priority,
            Status = userTaskDto.Status
        }));

        var result = await _taskService.CreateUserTask(userId, userTaskDto);
        
        var resultUserTaskDto = result.Match<UserTaskDto?>(s => s, f => null);

        Assert.NotNull(resultUserTaskDto);
        Assert.NotEqual(Guid.Empty,resultUserTaskDto.Id);
        Assert.Equal(taskId, resultUserTaskDto.Id);
        Assert.Equal(userTaskDto.Title, resultUserTaskDto.Title);
    }

    [Fact]
    public async Task GetUserTaskById_TaskNotFound_ReturnsTaskRetrievalFailed()
    {
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        _taskRepositoryMock.Setup(t => t.GetTasks()).Returns(new List<UserTask>().AsQueryable().BuildMock());

        var result = await _taskService.GetUserTaskById(userId, taskId);

        Assert.Equal($"There are no tasks with such id {taskId}", result.Match(s => "", f => f.ErrorMessage));
    }

    [Fact]
    public async Task GetUserTaskById_ForbiddenAccess_ReturnsTaskRetrievalFailed()
    {
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = new UserTask { Id = taskId, UserId = Guid.NewGuid() };
        _taskRepositoryMock.Setup(t => t.GetTasks()).Returns(new List<UserTask> { task }.AsQueryable().BuildMock());

        var result = await _taskService.GetUserTaskById(userId, taskId);

        Assert.Equal("Denied access to another user's task", result.Match(s => "", f => f.ErrorMessage));
    }

    [Fact]
    public async Task GetUserTaskById_Success_ReturnsUserTaskDto()
    {
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = new UserTask { Id = taskId, UserId = userId };
        _taskRepositoryMock.Setup(t => t.GetTasks()).Returns(new List<UserTask> { task }.AsQueryable().BuildMock());

        var result = await _taskService.GetUserTaskById(userId, taskId);
        
        Assert.Equal(taskId, result.Match(s => s.Id, f => Guid.Empty));
    }
}
