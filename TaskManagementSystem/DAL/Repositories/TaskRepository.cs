using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Repositories;

class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;
    
    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }
    
    public IQueryable<UserTask> GetTasks()
    {
        return _context.Tasks;
    }

    public async Task InsertTask(UserTask user)
    {
        _context.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTask(UserTask user)
    {
        _context.Remove(user);
        await _context.SaveChangesAsync();

    }

    public async Task UpdateTask(UserTask user)
    {
        _context.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserTask> GetTaskById(Guid id)
    {
        return await _context.Tasks.FindAsync(id);
    }
}