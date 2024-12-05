namespace TaskManagementSystem.BLL.Errors;

public abstract class TasksRetrievalFailed : ErrorBase
{
    public TasksRetrievalFailed(string errorMessage) : base(errorMessage)
    {
    }
}