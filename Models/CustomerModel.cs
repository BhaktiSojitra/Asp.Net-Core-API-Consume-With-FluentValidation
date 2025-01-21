using System.ComponentModel.DataAnnotations;

namespace API_Consume.Models
{
    public class CustomerModel
    {
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string HomeAddress { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string GSTNo { get; set; }
        public string CityName { get; set; }
        public string PinCode { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "The Net amount must be greater than 0 and must be a positive.")]
        public decimal NetAmount { get; set; }
        public int UserID { get; set; }
    }
}
