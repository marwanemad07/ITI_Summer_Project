using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class LoginViewModel
    {
        [MinLength(3), MaxLength(100)]
        [Remote("ValidUsername", "Login", ErrorMessage = "Not a valid username")]
        public string Username { get; set; }
        [MinLength(6, ErrorMessage = "Password must be at least 6 chars")]
        [Remote("ValidLogin", "Login", AdditionalFields = "Username" ,ErrorMessage = "Wrong Password")]
        public string Password { get; set; }
    }
}
