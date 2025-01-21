using API_Consume.Models;
using FluentValidation;

namespace API_Consume.Validators
{
    public class ProductValidator : AbstractValidator<ProductModel>
    {
        public ProductValidator()
        {
            RuleFor(model => model.ProductName)
                .NotNull().NotEmpty().WithMessage("Product name is required.")
                .Matches("^[a-zA-Z ]*$").WithMessage("Product Name should only contain letters.");

            RuleFor(model => model.ProductCode)
                .NotNull().NotEmpty().WithMessage("Product code is required.")
                .Matches("^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z0-9]*$").WithMessage("Product code must contain at least one letter and one number, and only alphanumeric characters are allowed.")
                .MinimumLength(4).WithMessage("Product code length should be 4 characters.")
                .MaximumLength(6).WithMessage("Product code length should not exceed 6 characters.");

            RuleFor(model => model.Description)
                .NotNull().NotEmpty().WithMessage("Description is required.")
                .Matches("^[a-zA-Z ]*$").WithMessage("Description should only contain letters.");

            RuleFor(model => model.UserID)
                .GreaterThan(0).WithMessage("User ID greater than 0 and must be a positive integer.");
        }
    }
}
