using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MinLength(2) ,MaxLength(100)]
        public string CategoryName { get; set; }
    }
}
