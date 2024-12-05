using FluentValidation;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.BLL.Validators;

public class UserTaskDtoValidator: AbstractValidator<UserTaskDto>
{
    public UserTaskDtoValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty()
            .MaximumLength(ValidationConstants.TaskTitleMaxLength);

        RuleFor(task => task.Description)
            .MaximumLength(ValidationConstants.TaskDescriptionMaxLength);
        
        RuleFor(task => task.DueData)
            .GreaterThanOrEqualTo(DateTime.Now).WithMessage("Due date cannot be in the past.")
            .When(task => task.DueData.HasValue);
        
        RuleFor(task => task.Status)
            .NotNull()
            .IsInEnum().WithMessage("Invalid status value.");

        RuleFor(task => task.Priority)
            .NotNull()
            .IsInEnum().WithMessage("Invalid priority value.");
    }
}