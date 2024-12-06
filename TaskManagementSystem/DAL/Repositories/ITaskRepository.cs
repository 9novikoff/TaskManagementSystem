using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Repositories;

public interface ITaskRepository
{
    public IQueryable<UserTask> GetTasks();
    public Task<UserTask> InsertTask(UserTask user);
    public Task DeleteTask(UserTask user);
    public Task<UserTask> UpdateTask(UserTask user);
    public Task<UserTask> GetTaskById(Guid id);
}