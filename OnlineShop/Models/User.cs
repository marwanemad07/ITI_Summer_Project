using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class User
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
        [MinLength(6, ErrorMessage = "Password must be at least 6 chars")]
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
