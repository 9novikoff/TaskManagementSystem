namespace TaskManagementSystem.BLL;

public readonly struct ServiceResult<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    public ServiceResult(TValue value)
    {
        _value = value;
        _error = default;
        IsError = false;
    }
    
    public ServiceResult(TError error)
    {
        _value = default;
        _error = error;
        IsError = true;
    }

    public ServiceResult(bool isError)
    {
        _value = default;
        _error = default;
        IsError = isError;
    }
    
    public bool IsError { get; }

    public static implicit operator ServiceResult<TValue, TError>(TValue value) => new (value);
    public static implicit operator ServiceResult<TValue, TError>(TError error) => new (error);
    
    public TResult Match<TResult>(Func<TValue, TResult> success, Func<TError, TResult> failure)
    {
        return !IsError ? success(_value) : failure(_error);
    }
}