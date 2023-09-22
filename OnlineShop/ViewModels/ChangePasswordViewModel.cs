using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int Id { get; set; }
        [MinLength(6, ErrorMessage = "Password must be at least 6 chars")]
        public string NewPassword { get; set; }
        [Remote("ValidConfirmPassword", "Customer", AdditionalFields = "NewPassword", ErrorMessage = "Password differs from ConfirmPassword")]
        public string ConfirmPassword { get; set;}

    }
}
