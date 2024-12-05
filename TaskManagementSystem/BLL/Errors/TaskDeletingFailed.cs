namespace TaskManagementSystem.BLL.Errors;

public class TaskDeletingFailed: ErrorBase
{
    public TaskDeletingFailed(string errorMessage) : base(errorMessage)
    {
    }
}