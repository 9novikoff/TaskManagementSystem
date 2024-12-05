using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.BLL.Errors;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.BLL.Services;

class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UserTaskDto> _taskValidator;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger, IValidator<UserTaskDto> taskValidator, IMapper mapper, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _logger = logger;
        _taskValidator = taskValidator;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<UserTaskDto, TaskCreationFailed>> CreateUserTask(Guid userId, UserTaskDto taskDto)
    {
        var validationResult = await _taskValidator.ValidateAsync(taskDto);

        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Failed task creation attempt with title {title}: Validation Failed", taskDto.Title);
            return new TaskCreationFailed(string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var user = await _userRepository.GetUsers().SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            _logger.LogInformation("Failed task creation attempt with title {title}: User doesn't exist", taskDto.Title);
            return new TaskCreationFailed("User doesn't exist");
        }
        
        var task = _mapper.Map<UserTask>(taskDto, opt => opt.AfterMap((_, dest) => dest.User = user));

        await _taskRepository.InsertTask(task);
        
        _logger.LogInformation("Task was successfully created with title {title}", taskDto.Title);

        return _mapper.Map<UserTaskDto>(task);
    }

    public async Task<ServiceResult<List<UserTaskDto>, TasksRetrievalFailed>> GetAllUserTasks(Guid userId, TaskFilter filter, TaskSort sort, TaskPagination pagination)
    {
        var userTasksQuery = _taskRepository.GetTasks().Where(t => t.UserId == userId);
        
        if (filter.Status.HasValue)
            userTasksQuery = userTasksQuery.Where(t => t.Status == filter.Status);

        if (filter.Priority.HasValue)
            userTasksQuery = userTasksQuery.Where(t => t.Priority == filter.Priority);

        if (filter.DateTo.HasValue)
            userTasksQuery = userTasksQuery.Where(t => t.DueDate <= filter.DateTo);

        if (filter.DateFrom.HasValue)
            userTasksQuery = userTasksQuery.Where(t => t.DueDate >= filter.DateFrom);
        
        if (!string.IsNullOrWhiteSpace(sort.SortColumn))
        {
            userTasksQuery = sort.SortColumn.ToLower() switch
            {
                "duedate" => sort.IsDesc ? userTasksQuery.OrderByDescending(t => t.DueDate) : 
                    userTasksQuery.OrderBy(t => t.DueDate),
                "priority" => sort.IsDesc ? userTasksQuery.OrderByDescending(t => t.Priority) : 
                    userTasksQuery.OrderBy(t => t.Priority),
                "status" => sort.IsDesc ? userTasksQuery.OrderByDescending(t => t.Status) : 
                    userTasksQuery.OrderBy(t => t.Status),
                _ => userTasksQuery
            };
        }
        
        if (pagination.PageNumber != null && pagination.PageSize != null)
        {
            userTasksQuery = userTasksQuery
                .Skip((pagination.PageNumber.Value - 1) * pagination.PageSize.Value)
                .Take(pagination.PageNumber.Value);
        }

        var tasks = await userTasksQuery.ToListAsync();
        
        return _mapper.Map<List<UserTaskDto>>(tasks);
    }

    public async Task<ServiceResult<UserTaskDto, TaskRetrievalFailed>> GetUserTaskById(Guid userId, Guid taskId)
    {
        var task = await _taskRepository.GetTasks().SingleOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            _logger.LogInformation("Failed task get attempt with id {id}: There is no such id", taskId);
            return new TaskRetrievalFailed($"There are no tasks with such id {taskId}");
        }

        if (task.UserId != userId)
        {
            _logger.LogInformation("Failed task get attempt with id {id}: Task related to another user", taskId);
            return new TaskRetrievalFailed("Denied access to another user's task");
        }
        
        _logger.LogInformation("Successful task get with id {id}", taskId);
        return _mapper.Map<UserTaskDto>(task);
    }

    public async Task<ServiceResult<UserTaskDto, TaskUpdatingFailed>> UpdateUserTask(Guid userId, Guid taskId, UserTaskDto taskDto)
    {
        var validationResult = await _taskValidator.ValidateAsync(taskDto);

        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Failed task update attempt with title {title}: Validation Failed", taskDto.Title);
            return new TaskUpdatingFailed(string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var task = _mapper.Map<UserTask>(taskDto, opt => opt.AfterMap((_, dest) => dest.Id = taskId));
        
        if (task.UserId != userId)
        {
            _logger.LogInformation("Failed task update attempt with id {id}: Task related to another user", taskId);
            return new TaskUpdatingFailed("Denied access to another user's task");
        }

        await _taskRepository.UpdateTask(task);
        
        _logger.LogInformation("Successful task update with id {id}", task.Id);

        return _mapper.Map<UserTaskDto>(task);
    }

    public async Task<ServiceResult<bool, TaskDeletingFailed>> DeleteUserTask(Guid userId, Guid taskId)
    {
        var task = await _taskRepository.GetTasks().SingleOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            _logger.LogInformation("Failed task delete attempt with id {id}: There is no such id", taskId);
            return new TaskDeletingFailed($"There are no tasks with such id {taskId}");
        }
        
        if (task.UserId != userId)
        {
            _logger.LogInformation("Failed task delete attempt with id {id}: Task related to another user", taskId);
            return new TaskDeletingFailed("Denied access to another user's task");
        }

        await _taskRepository.DeleteTask(task);
        
        _logger.LogInformation("Successful task deletion with id {id}", taskId);
        
        return true;
    }
}