namespace TaskManagementSystem.BLL.Errors;

public class TasksRetrievalFailed : ErrorBase
{
    public TasksRetrievalFailed(string errorMessage) : base(errorMessage)
    {
    }
}