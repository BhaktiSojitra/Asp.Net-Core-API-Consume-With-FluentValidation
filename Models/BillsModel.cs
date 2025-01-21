using System.ComponentModel.DataAnnotations;
namespace API_Consume.Models
{
    public class BillsModel
    {
        public int? BillID { get; set; }
        public string BillNumber { get; set; }
        public DateTime BillDate { get; set; }
        public int OrderID { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "The Total amount must be greater than 0 and must be a positive.")]
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "The Net amount must be greater than 0 and must be a positive.")]
        public decimal NetAmount { get; set; }
        public int UserID { get; set; }
    }
}
