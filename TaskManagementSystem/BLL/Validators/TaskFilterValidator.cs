using FluentValidation;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.BLL.Validators;

public class TaskFilterValidator : AbstractValidator<TaskFilter>
{
    public TaskFilterValidator()
    {
        RuleFor(filter => filter.DateFrom)
            .LessThanOrEqualTo(filter => filter.DateTo)
            .When(filter => filter.DateFrom.HasValue && filter.DateTo.HasValue)
            .WithMessage("DateFrom must be earlier than or equal to DateTo.");
    }
}