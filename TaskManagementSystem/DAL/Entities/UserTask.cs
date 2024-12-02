﻿namespace TaskManagementSystem.DAL.Entities;

public class UserTask : EntityBase
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueData { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    
    public User User { get; set; }
}