using FluentValidation;
using TaskManagementSystem.DTO;


namespace TaskManagementSystem.BLL.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().MinimumLength(ValidationConstants.UsernameOrEmailMinLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(ValidationConstants.PasswordMinLength)
            .MaximumLength(ValidationConstants.PasswordMaxLength)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}