using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class CustomerEditProfileViewModel
    {
        public int Id { get; set; }
        [MinLength(3), MaxLength(100)]
        [Remote("ValidUsername", "Customer", ErrorMessage = "Username already exists")]
        public string Username { get; set; }
        [Range(18, 60, ErrorMessage = "Age must between 18 and 60")]
        public int Age { get; set; }
        public IFormFile Image { get; set; } = default!;
    }
}
