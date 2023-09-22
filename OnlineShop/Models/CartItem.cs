using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class CartItem
    {
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [MinLength(1)]
        public int Quantity { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}
