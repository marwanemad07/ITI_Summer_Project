using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        [MinLength(1)]
        public decimal UnitPrice { get; set; }
        [MinLength(0)]
        public int UnitsInStock { get; set; }
        public string Image { get; set; } = "Default.jpg";
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
