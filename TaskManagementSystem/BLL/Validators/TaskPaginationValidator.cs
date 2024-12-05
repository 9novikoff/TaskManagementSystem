using FluentValidation;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.BLL.Validators;

public class TaskPaginationValidator : AbstractValidator<TaskPagination>
{
    public TaskPaginationValidator()
    {
        RuleFor(pagination => pagination.PageSize)
            .GreaterThan(0)
            .When(pagination => pagination.PageSize.HasValue)
            .WithMessage("PageSize must be greater than 0.");

        RuleFor(pagination => pagination.PageNumber)
            .GreaterThan(0)
            .When(pagination => pagination.PageNumber.HasValue)
            .WithMessage("PageNumber must be greater than 0.");

        RuleFor(pagination => pagination)
            .Must(pagination => pagination.PageSize.HasValue == pagination.PageNumber.HasValue)
            .WithMessage("Both PageSize and PageNumber must be provided together.");
    }
}