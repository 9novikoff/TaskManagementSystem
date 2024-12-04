namespace TaskManagementSystem.BLL.Errors;

public class LoginFailed : ErrorBase
{
    public LoginFailed(string errorMessage) : base(errorMessage)
    {
    }
}