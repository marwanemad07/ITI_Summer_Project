using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class RegisterViewModel
    {
        [MinLength(3), MaxLength(100)]
        [Remote("UniqueUsername", "Register", ErrorMessage = "Username exists already!")]
        public string Username { get; set; }
        [Range(18, 60, ErrorMessage = "Age must be between 18 and 60")]
        public int Age { get; set; }
        [MinLength(6, ErrorMessage = "Password must be at least 6 chars")]
        public string Password { get; set; }
        [Remote("ValidConfirmPassword", "Register", AdditionalFields = "Password", ErrorMessage = "Password differs from ConfirmPassword")]
        public string ConfirmPassword { get; set; }

    }
}
