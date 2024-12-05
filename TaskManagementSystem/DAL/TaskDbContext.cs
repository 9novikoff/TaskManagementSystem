using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
        
    }
    
    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically updates the CreatedAt and UpdatedAt timestamps for tracked entities.
    /// - Sets CreatedAt and UpdatedAt to the current UTC time for newly added entities.
    /// - Updates UpdatedAt to the current UTC time for modified entities.
    /// This method should be called before saving changes to ensure timestamps are accurate.
    /// </summary>
    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow;

            if (entity.State == EntityState.Added)
            {
                entity.Entity.CreatedAt = now;
            }
            entity.Entity.UpdatedAt = now;
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserTask> Tasks { get; set; }
}