namespace TaskManagementSystem.BLL.Errors;

public class TaskCreationFailed: ErrorBase
{
    public TaskCreationFailed(string errorMessage) : base(errorMessage)
    {
    }
}