using FluentValidation;
using TieChef.Models.DTOs;

namespace TieChef.Validators
{
    public class StaffDTOValidator : AbstractValidator<StaffDTO>
    {
        public StaffDTOValidator()
        {
            RuleFor(x => x.fullName)
                .NotEmpty().WithMessage("Full Name is required")
                .Length(2, 100).WithMessage("Full Name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Full Name must contain only letters and spaces");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

            RuleFor(x => x.phoneNumber)
                .NotEmpty().WithMessage("Phone Number is required")
                .GreaterThan(100000).WithMessage("Phone Number must be valid");

            RuleFor(x => x.salary)
                .GreaterThan(0).WithMessage("Salary must be greater than 0")
                .PrecisionScale(18, 2, false).WithMessage("Salary cannot have more than 2 decimal places");

            RuleFor(x => x.KPI)
                .MaximumLength(500).WithMessage("KPI must not exceed 500 characters");
        }
    }
}
