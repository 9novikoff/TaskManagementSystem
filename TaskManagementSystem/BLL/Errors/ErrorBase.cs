namespace TaskManagementSystem.BLL.Errors;

public abstract class ErrorBase
{
    public ErrorBase(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
    public string ErrorMessage { get; }
}