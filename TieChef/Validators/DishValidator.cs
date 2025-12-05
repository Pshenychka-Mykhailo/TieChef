using FluentValidation;
using TieChef.Models.Entities;

namespace TieChef.Validators
{
    public class DishValidator : AbstractValidator<Dish>
    {
        public DishValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Dish Name is required")
                .Length(2, 100).WithMessage("Dish Name must be between 2 and 100 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .PrecisionScale(18, 2, false).WithMessage("Price cannot have more than 2 decimal places");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
        }
    }
}
