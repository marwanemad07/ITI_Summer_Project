using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        [MinLength(3), MaxLength(100)]
        public string Username { get; set; }
        [Range(18, 60)]
        public int Age { get; set; }
        [MinLength(0)]
        [DataType(DataType.Currency)]
        public decimal Money { get; set; } = 1000.0M;
        public string Image { get; set; } = "Default.jpg";
    }
}
