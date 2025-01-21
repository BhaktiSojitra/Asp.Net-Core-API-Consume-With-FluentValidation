using API_Consume.Models;
using FluentValidation;

namespace API_Consume.Validators
{
    public class OrderDetailValidator : AbstractValidator<OrderDetailModel>
    {
        public OrderDetailValidator() 
        {
            RuleFor(model => model.OrderID)
                .GreaterThan(0).WithMessage("Order ID greater than 0 and must be a positive integer.");

            RuleFor(model => model.ProductID)
                .GreaterThan(0).WithMessage("Product ID greater than 0 and must be a positive integer.");

            RuleFor(model => model.UserID)
                .GreaterThan(0).WithMessage("User ID greater than 0 and must be a positive integer.");
        }   
    }
}
