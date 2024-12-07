using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        using var context = serviceScope.ServiceProvider.GetRequiredService<DAL.TaskDbContext>();
        context.Database.Migrate();
    }
}