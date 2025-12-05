using FluentValidation;
using TieChef.Models.DTOs;

namespace TieChef.Validators
{
    public class ReceiptDTOValidator : AbstractValidator<ReceiptDTO>
    {
        public ReceiptDTOValidator()
        {
            RuleFor(x => x.tableId).GreaterThan(0).WithMessage("Table ID must be greater than 0");
            
            RuleFor(x => x.staffId)
                .GreaterThan(0).When(x => x.staffId.HasValue).WithMessage("Staff ID must be greater than 0");

            RuleFor(x => x.sum)
                .GreaterThanOrEqualTo(0).WithMessage("Sum cannot be negative")
                .PrecisionScale(18, 2, false).WithMessage("Sum cannot have more than 2 decimal places");

            RuleFor(x => x.dishIds)
                .NotEmpty().When(x => x.wasPaid).WithMessage("Paid receipt must contain dishes");
        }
    }
}
