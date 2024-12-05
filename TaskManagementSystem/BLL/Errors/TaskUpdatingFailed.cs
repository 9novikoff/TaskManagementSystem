namespace TaskManagementSystem.BLL.Errors;

public class TaskUpdatingFailed: ErrorBase
{
    public TaskUpdatingFailed(string errorMessage) : base(errorMessage)
    {
    }
}