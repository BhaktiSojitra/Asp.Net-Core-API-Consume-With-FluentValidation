using API_Consume.Models;
using FluentValidation;

namespace API_Consume.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerModel>
    {
        public CustomerValidator() 
        {
            RuleFor(model => model.CustomerName)
                .NotNull().NotEmpty().WithMessage("Customer name is required.")
                .Matches("^[A-Za-z ]+$").WithMessage("Customer name must contain only letters and spaces.");

            RuleFor(model => model.HomeAddress)
                .NotNull().NotEmpty().WithMessage("Home address is required.")
                .MaximumLength(250).WithMessage("Home address must not exceed 250 characters.");

            RuleFor(model => model.Email)
                .NotNull().NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please Enter Valid Email");

            RuleFor(model => model.MobileNo)
                .NotNull().NotEmpty().WithMessage("Mobile number is required.")
                .Length(10).WithMessage("Mobile number should be 10 digit");

            RuleFor(model => model.GSTNo)
                .NotNull().NotEmpty().WithMessage("GST number is required.")
                .MaximumLength(15).WithMessage("Length should be 15.")
                .Matches("^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z0-9]*$").WithMessage("GST number must contain at least one letter and one number, and only alphanumeric characters are allowed.");

            RuleFor(model => model.CityName)
                .NotNull().NotEmpty().WithMessage("City name is required.")
                .Matches("^[A-Za-z]+$").WithMessage("City name must contain only letters.");

            RuleFor(model => model.PinCode)
                .NotNull().NotEmpty().WithMessage("Pin code is required.")
                .Length(6).WithMessage("Pin code should be 6 digit")
                .Matches("^[0-9]+$").WithMessage("Pin code must contain only numbers.");

            RuleFor(model => model.UserID)
                .GreaterThan(0).WithMessage("User ID greater than 0 and must be a positive integer.");
        }
    }
}
