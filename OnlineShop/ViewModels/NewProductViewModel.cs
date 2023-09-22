using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class NewProductViewModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [DisplayName("Product Name")]
        public string Name { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public IFormFile Image { get; set; } = default!;
        public string? CategoryName { get; set; }
        [DisplayName("Category Name")]
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem> CategoriesList { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
