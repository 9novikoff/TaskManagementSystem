namespace TaskManagementSystem.DAL.Entities;

public class User : EntityBase
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    
    public List<UserTask> Tasks { get; set; }
}