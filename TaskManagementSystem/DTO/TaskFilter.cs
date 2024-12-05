using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DTO;

public class TaskFilter
{
    public Status? Status { get; set; }
    public Priority? Priority { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}