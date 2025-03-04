﻿using API_Consume.Models;
using FluentValidation;

namespace API_Consume.Validators
{
    public class OrderValidator : AbstractValidator<OrderModel>
    {
        public OrderValidator() 
        {
            RuleFor(model => model.PaymentMode)
                .NotNull().NotEmpty().WithMessage("Payment mode is required.");

            RuleFor(model => model.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than 0.");

            RuleFor(model => model.ShippingAddress)
                .NotNull().NotEmpty().WithMessage("Shipping Address is required.");
        }
    }
}
