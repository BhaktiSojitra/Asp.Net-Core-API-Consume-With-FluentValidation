using API_Consume.Models;
using FluentValidation;

namespace API_Consume.Validators
{
    public class BillsValidator : AbstractValidator<BillsModel>
    {
        public BillsValidator() 
        {
            RuleFor(model => model.BillNumber)
                .NotNull().NotEmpty().WithMessage("Bill number is required.")
                .Matches("^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z0-9]*$").WithMessage("Bill number must contain at least one letter and one number, and only alphanumeric characters are allowed.")
                .MinimumLength(4).WithMessage("Bill number length should be 4 characters.")
                .MaximumLength(6).WithMessage("Bill number length should not exceed 6 characters.");

            RuleFor(model => model.Discount)
                .GreaterThan(0).WithMessage("Discount must be greater than 0.");
        }
    }
}
