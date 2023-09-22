using Microsoft.AspNetCore.Mvc;
using OnlineShop.Entites;
using OnlineShop.ViewModels;
using OnlineShop.Models;
namespace OnlineShop.Controllers
{
    public class RegisterController : Controller
    {
        ApplicationDbContext DB = new ApplicationDbContext();
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreateAccount(RegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
                User NewUser = new User
                {
                    Username = user.Username,
                    Age = user.Age,
                    Password = user.Password,
                };
                DB.Users.Add(NewUser);
                DB.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View("Index", user);
        }

        //Remote validations
        public IActionResult UniqueUsername(string username)
        {
            if (DB.Users.Where(U => U.Username == username).Any())
            {
                return Json(false);
            }
            return Json(true);
        }
        public IActionResult ValidConfirmPassword(string ConfirmPassword, string Password) 
        {
            if (ConfirmPassword.Equals(Password)) return Json(true);
            return Json(false);
        }

    }
}
