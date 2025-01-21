using System.ComponentModel.DataAnnotations;

namespace API_Consume.Models
{
    public class OrderModel
    {
        public int? OrderID { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "The Order number must be greater than 0 and must be a positive.")]
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerID { get; set; }
        public string? PaymentMode { get; set; }
        public decimal? TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public int UserID { get; set; }
    }

    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
