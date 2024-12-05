using FluentValidation;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.BLL.Validators;

public class TaskSortValidator : AbstractValidator<TaskSort>
{
    private static readonly string[] ValidSortColumns = { "duedate", "priority", "status" };

    public TaskSortValidator()
    {
        RuleFor(sort => sort.SortColumn)
            .Must(column => ValidSortColumns.Contains(column))
            .When(column => !string.IsNullOrEmpty(column.SortColumn))
            .WithMessage($"SortColumn must be one of the following: {string.Join(", ", ValidSortColumns)}");
    }
}