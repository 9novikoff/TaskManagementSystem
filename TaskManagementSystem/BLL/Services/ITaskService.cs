using TaskManagementSystem.BLL.Errors;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.BLL.Services;

public interface ITaskService
{
    public Task<ServiceResult<UserTaskDto, TaskCreationFailed>> CreateUserTask(Guid userId, CreateUserTaskDto userTaskDto);
    public Task<ServiceResult<List<UserTaskDto>, TasksRetrievalFailed>> GetAllUserTasks(Guid userId, TaskFilter filter, 
        TaskSort sort, TaskPagination pagination);
    public Task<ServiceResult<UserTaskDto, TaskRetrievalFailed>> GetUserTaskById(Guid userId, Guid taskId);
    public Task<ServiceResult<UserTaskDto, TaskUpdatingFailed>> UpdateUserTask(Guid userId, Guid taskId, CreateUserTaskDto taskDto);
    public Task<ServiceResult<bool, TaskDeletingFailed>> DeleteUserTask(Guid userId, Guid taskId);
}