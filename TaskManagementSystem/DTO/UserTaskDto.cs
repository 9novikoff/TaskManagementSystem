﻿using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DTO;

public class UserTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Status? Status { get; set; }
    public Priority? Priority { get; set; }
}