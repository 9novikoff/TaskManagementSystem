namespace TaskManagementSystem.BLL.Errors;

public class TaskRetrievalFailed: ErrorBase
{
    public TaskRetrievalFailed(string errorMessage) : base(errorMessage)
    {
    }
}