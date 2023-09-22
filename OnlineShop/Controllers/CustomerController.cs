using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Entites;
using OnlineShop.Models;
using OnlineShop.ViewModels;
using System.ComponentModel.Design;

namespace OnlineShop.Controllers
{
    public class CustomerController : Controller
    {
        ApplicationDbContext DB = new ApplicationDbContext();
        IWebHostEnvironment _WebHostEnvironment;
        public CustomerController(IWebHostEnvironment webHost)
        {
            _WebHostEnvironment = webHost;
        }
        public IActionResult Index()
        {
            ViewBag.Id = int.Parse(Request?.Cookies["UserId"]??"0");
            List<Product> Lst = DB.Products.ToList();
            return View(Lst);
        }
        public IActionResult Details(int id)
        {
            //Check If this item was deleted now from admin (this product isn't available anymore)
            Product? product = DB.Products.Find(id);
            if(product == null)
            {
                List<Product> Lst = DB.Products.ToList();
                return View(nameof(Index),Lst);
            }
            string categoryName = DB.Categories.Find(product?.CategoryId ?? 0)?.CategoryName ?? "NA";
            return View(new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Image = product.Image,
                CategoryName = categoryName,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock
            });
        }

        public IActionResult ShowProfile([FromRoute] int id)
        {
            CustomerViewModel? Data = DB.Users.Where(U => U.Id == id).
                                        Select(U => new CustomerViewModel {
                                            Id = U.Id,
                                            Username = U.Username,
                                            Age = U.Age,
                                            Image = U.Image,
                                            Money = U.Money,
                                        }).FirstOrDefault();
            if (Data == null)
            {
                List<Product> Lst = DB.Products.ToList();
                ViewBag.Id = int.Parse(Request?.Cookies["UserId"] ?? "0");
                return View("Index", Lst);
            }
            return View(Data);
        }
        [HttpGet]
        public IActionResult EditProfile(int id)
        {
            CustomerEditProfileViewModel C = DB.Users.Where(U => U.Id == id).
                Select(U => new CustomerEditProfileViewModel {
                    Id = U.Id,
                    Username = U.Username,
                    Age = U.Age,
                }).First();
            
            return View(C);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditProfile(CustomerEditProfileViewModel C, [FromRoute] int id)
        {
            if (ModelState.IsValid)
            {
                var ImageName = $"{Guid.NewGuid()}{Path.GetExtension(C.Image.FileName)}";
                var _path = $"{_WebHostEnvironment.WebRootPath}/Images";
                var path = Path.Combine(_path, ImageName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await C.Image.CopyToAsync(stream);
                }
                User OldUser = DB.Users.Find(C.Id);
                OldUser.Age = C.Age;
                OldUser.Image = ImageName;
                OldUser.Username = C.Username;
                DB.Users.Update(OldUser);
                DB.SaveChanges();
                CustomerViewModel customer = new CustomerViewModel
                {
                    Id = OldUser.Id,
                    Username = OldUser.Username,
                    Age = OldUser.Age,
                    Image = OldUser.Image,
                    Money = OldUser.Money
                };
                ViewBag.Id = OldUser.Id;
                return RedirectToAction("ShowProfile", customer);
            }
            return View(C);
        }
        [HttpGet]
        public IActionResult ChangePassword(int id)
        {
            ChangePasswordViewModel model = DB.Users.Where(U => U.Id == id).
                Select(U => new ChangePasswordViewModel { Id = U.Id }).First();
            return View(model);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User OldUser = DB.Users.Find(model.Id);
                OldUser.Password = model.NewPassword;
                DB.Users.Update(OldUser);
                DB.SaveChanges();
                ViewBag.Id = OldUser.Id;
                CustomerViewModel customer = new CustomerViewModel
                {
                    Id = OldUser.Id,
                    Username = OldUser.Username,
                    Age = OldUser.Age,
                    Image = OldUser.Image,
                    Money = OldUser.Money
                };
                return RedirectToAction("ShowProfile", customer);
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult AddToCart([FromRoute] int id)
        {
            //Check If this item was deleted now from admin
            Product? product = DB.Products.Find(id);
            if (product == null)
            {
                List<Product> PLst = DB.Products.ToList();
                return View(nameof(Index), PLst);
            }

            User UserData = DB.Users.Find(int.Parse(Request?.Cookies["UserId"] ?? "0"));
            int Id = UserData?.Id ?? 0;
            ViewBag.Id = Id;
            CheckActiveCart(Id);
            int CurrentCartId = DB.Carts.Where(C => C.UserId == Id && C.Active == true).First().Id;
            if (DB.CartItems.Where(C => C.CartId == CurrentCartId && C.ProductId == id).Any())
            {
                CartItem CI = DB.CartItems.Where(C => C.CartId == CurrentCartId && C.ProductId == id).First();
                CI.Quantity++;
                DB.CartItems.Update(CI);
            }
            else
            {
                DB.CartItems.Add(new CartItem { CartId = CurrentCartId, ProductId = id, Quantity = 1 });
            }
            DB.SaveChanges();
            List<Product> Lst = DB.Products.ToList();
            return View("Index", Lst);
        }
        [HttpGet]
        public IActionResult Cart([FromRoute] int id)
        {

            User UserData = DB.Users.Find(int.Parse(Request?.Cookies["UserId"] ?? "0"));
            id = UserData?.Id ?? 0;
            CheckActiveCart(id);
            int CurrenCartId = DB.Carts.Where(C => C.UserId == id && C.Active == true).First().Id;
            ViewBag.CartId = CurrenCartId;
            return View(GetListOfCartItems(CurrenCartId));
        }
        public IActionResult DecreaseItem(int ProductId, int CartId)
        {
            CartItem OldItem = DB.CartItems.Where(C=> C.ProductId == ProductId && C.CartId == CartId).First();
            if(OldItem.Quantity == 1)
            {
                DB.CartItems.Remove(OldItem);
            }
            else
            {
                OldItem.Quantity--;
                DB.CartItems.Update(OldItem);
            }
            DB.SaveChanges();
            ViewBag.CartId = CartId;
            return View("Cart", GetListOfCartItems(CartId));
        }
        public IActionResult Checkout(int CartId)
        {
            User? UserData = DB.Users.Find(int.Parse(Request?.Cookies["UserId"] ?? "0"));
            var Lst = GetListOfCartItems(CartId);
            decimal TotalPrice = 0;
            bool EnoughInStock = true;
            foreach(var C in Lst)
            {
                TotalPrice += C.Price * C.Quantity;
                if (C.Quantity > C.InStock) EnoughInStock = false;
            }
            if(TotalPrice <= UserData.Money && EnoughInStock)
            {
                var OldCart = DB.Carts.Where(C => C.Id == CartId).First();
                OldCart.Active = false;
                DB.Carts.Update(OldCart);
                UserData.Money -= TotalPrice;
                DB.Users.Update(UserData);
                foreach(var C in Lst)
                {
                    var OldProduct = DB.Products.Find(C.ProductId);
                    OldProduct.UnitsInStock -= C.Quantity;
                    DB.Products.Update(OldProduct);
                }
                DB.SaveChanges();
                CheckActiveCart(UserData.Id);
            }
            else
            {
                // do some logic here if a the user money aren't enough or he needs quantity of
                // a product that not available in stock
            }
            int CurrentCartId = DB.Carts.Where(C => UserData.Id == C.UserId && C.Active == true).First().Id;
            ViewBag.CartId = CurrentCartId;
            return View("Cart", GetListOfCartItems(CurrentCartId));
        }
        private void CheckActiveCart(int id)
        {
            if (!DB.Carts.Where(C => C.UserId == id && C.Active == true).Any())
            {
                DB.Carts.Add(new Cart { UserId = id, Active = true });
                DB.SaveChanges();
            }
        }
        private List<CartItemsViewModel> GetListOfCartItems(int CurrenCartId)
        {
            var ProductIds = DB.CartItems.Where(C => C.CartId == CurrenCartId).Select(C => new { Id = C.ProductId, Qty = C.Quantity }).ToList();
            List<CartItemsViewModel> Lst = new List<CartItemsViewModel>();
            foreach (var P in ProductIds)
            {
                Lst.Add(new CartItemsViewModel
                {
                    CartId = CurrenCartId,
                    ProductId = P.Id,
                    Quantity = P.Qty,
                    ProductName = DB.Products.Find(P.Id)?.Name ?? "NA",
                    Price = DB.Products.Find(P.Id)?.UnitPrice ?? 1000000.0M,
                    Image = DB.Products.Find(P.Id)?.Image ?? "Default.jpg",
                    InStock = DB.Products.Find(P.Id)?.UnitsInStock ?? 0
                }) ;
            }
            return Lst;
        }
        //Remote Validations
        public IActionResult ValidUsername(string UserName)
        {
            int Id = int.Parse(Request.Cookies["UserId"]);
            string CurrentUsername = DB.Users.Find(Id).Username;
            if (!DB.Users.Where(U => U.Username == UserName).Any() || UserName == CurrentUsername)
                return Json(true);
            return Json(false);
        }
        public IActionResult ValidConfirmPassword(string ConfirmPassword, string NewPassword)
        {
            if (ConfirmPassword.Equals(NewPassword)) return Json(true);
            return Json(false);
        }

    }
}
