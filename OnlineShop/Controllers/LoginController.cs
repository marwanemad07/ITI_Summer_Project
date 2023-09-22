using Microsoft.AspNetCore.Mvc;
using OnlineShop.Entites;
using OnlineShop.ViewModels;
using OnlineShop.Models;
namespace OnlineShop.Controllers
{
    public class LoginController : Controller
    {
        ApplicationDbContext DB = new ApplicationDbContext();
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UserLogin(LoginViewModel user)
        {
            User UserData = DB.Users.Where(U => U.Username == user.Username).First();
            CookieOptions options = new CookieOptions { Expires = DateTime.Now.AddHours(1) };
            Response.Cookies.Append("UserId", UserData.Id.ToString(), options);
            if (UserData.IsAdmin)
            {
                return RedirectToAction("Index", "Admin",
                    new AdminViewModel {
                        Id = UserData.Id,
                        Username = UserData.Username,
                    });
            }
            CustomerViewModel Customer = new CustomerViewModel
            {
                Id = UserData.Id,
                Username = UserData.Username,
            };
            return RedirectToAction("Index", "Customer", Customer); 
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserId");
            return RedirectToAction("Index", "Home");
        }


        //remote validations
        public IActionResult ValidUsername(string username)
        {
            if (DB.Users.Where(U => U.Username == username).Any()) 
                return Json(true);
            return Json(false);
        }
        public IActionResult ValidLogin(string password, string username)
        {
            if (DB.Users.Where(U => U.Username == username && U.Password == password).Any())
                return Json(true);
            return Json(false);
        }
    }
}
