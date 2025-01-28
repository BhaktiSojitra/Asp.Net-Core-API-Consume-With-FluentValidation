using API_Consume.Models;
using FluentValidation;

namespace API_Consume.Validators
{
    public class OrderDetailValidator : AbstractValidator<OrderDetailModel>
    {
        public OrderDetailValidator() 
        {
        }   
    }
}
