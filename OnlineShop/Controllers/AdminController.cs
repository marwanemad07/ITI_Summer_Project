using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Entites;
using OnlineShop.Models;
using OnlineShop.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace OnlineShop.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext DB = new ApplicationDbContext();
        IWebHostEnvironment _WebHostEnvironment;
        public AdminController(IWebHostEnvironment webHost)
        {
            _WebHostEnvironment = webHost;
        }
        public IActionResult Index()
        {
            List<Product> Lst = DB.Products.ToList();
            return View(Lst);
        }
        public IActionResult Details(int id)
        {
            Product product = DB.Products.Find(id);
            string categoryName = DB.Categories.Find(product?.CategoryId??0).CategoryName;
            return View(new ProductViewModel {
                Id = product.Id,
                Name = product.Name,
                Image = product.Image,
                CategoryName = categoryName,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock
            }) ;
        }

        [HttpGet]
        public IActionResult Edit(int id) 
        {
            Product product = DB.Products.Find(id);
            NewProductViewModel NewProduct = new NewProductViewModel
            {
                Id = product.Id,
                Name= product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                CategoryId = product.CategoryId
            }; 
            ViewBag.action = "Edit";
            ViewBag.Title = "Edit-Product";
            ViewBag.Id = id;
            NewProduct.CategoriesList = DB.Categories.Select(C => new SelectListItem { Value = C.Id.ToString(), Text = C.CategoryName }).OrderBy(C => C.Text).ToList(); ;
            return View(NewProduct);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(NewProductViewModel NewProduct, [FromRoute] int id) 
        {
            if(ModelState.IsValid)
            {
                var ImageName = $"{Guid.NewGuid()}{Path.GetExtension(NewProduct.Image.FileName)}";
                var _path = $"{_WebHostEnvironment.WebRootPath}/Images";
                var path = Path.Combine(_path, ImageName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await NewProduct.Image.CopyToAsync(stream);
                }
                DB.Products.Update(new Product 
                {
                    Id = NewProduct.Id,
                    Name = NewProduct.Name,
                    Image = ImageName,
                    Description = NewProduct.Description,
                    UnitPrice = NewProduct.UnitPrice,
                    UnitsInStock = NewProduct.UnitsInStock,
                    CategoryId = NewProduct.CategoryId 
                });
                DB.SaveChanges();
                List<Product> Lst = DB.Products.ToList();
                return View("Index", Lst);
            }
            return View(NewProduct);
        }
        public IActionResult New()
        {
            NewProductViewModel NewProduct = new NewProductViewModel();
            ViewBag.action = "New";
            ViewBag.Title = "New-Product";
            ViewBag.Id = 0;
            NewProduct.CategoriesList = DB.Categories.Select(C => new SelectListItem { Value = C.Id.ToString(), Text = C.CategoryName }).OrderBy(C => C.Text).ToList(); ;
            return View(NewProduct);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewProductViewModel NewProduct)
        {
            if (ModelState.IsValid)
            {
                var ImageName = $"{Guid.NewGuid()}{Path.GetExtension(NewProduct.Image.FileName)}";
                var _path = $"{_WebHostEnvironment.WebRootPath}/Images";
                var path = Path.Combine(_path, ImageName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await NewProduct.Image.CopyToAsync(stream);
                }
                DB.Products.Add(new Product
                {
                    Id = NewProduct.Id,
                    Name = NewProduct.Name,
                    Image = ImageName,
                    Description = NewProduct.Description,
                    UnitPrice = NewProduct.UnitPrice,
                    UnitsInStock = NewProduct.UnitsInStock,
                    CategoryId = NewProduct.CategoryId
                });
                DB.SaveChanges();
                List<Product> Lst = DB.Products.ToList();
                return View("Index", Lst);
            }
            return View(NewProduct);
        }

        public IActionResult Delete(int id)
        {
            Product? ToDelete = DB.Products.Find(id);
            if (ToDelete != null) DB.Products.Remove(ToDelete);
            DB.SaveChanges();
            List<Product> Lst = DB.Products.ToList();
            return RedirectToAction("Index", Lst);
        }
    }
}
