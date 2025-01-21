using System.ComponentModel.DataAnnotations;
namespace API_Consume.Models
{
    public class ProductModel
    {
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "The Product price must be greater than 0 and must be a positive.")]
        public double ProductPrice { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
    }

    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
