using System.ComponentModel.DataAnnotations;

namespace API_Consume.Models
{
    public class OrderDetailModel
    {
        public int? OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "The Quantity must be greater than 0 and must be a positive")]
        public int Quantity { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "The Amount must be greater than 0 and must be a positive.")]
        public decimal Amount { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "The Total amount must be greater than 0 and must be a positive.")]
        public decimal TotalAmount { get; set; }
        public int UserID { get; set; }
    }

    public class ProductDropDownModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }

    public class OrderDropDownModel
    {
        public int OrderID { get; set; }
        public int OrderNumber { get; set; }
    }
}
