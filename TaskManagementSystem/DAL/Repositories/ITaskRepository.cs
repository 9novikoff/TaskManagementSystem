using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Repositories;

public interface ITaskRepository
{
    public IQueryable<UserTask> GetTasks();
    public Task InsertTask(UserTask user);
    public Task DeleteTask(UserTask user);
    public Task UpdateTask(UserTask user);
    public Task<UserTask> GetTaskById(Guid id);
}