using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class AdminViewModel
    {
        public int Id { get; set; }
        [MinLength(3), MaxLength(100)]
        public string Username { get; set; } 
    }
}
