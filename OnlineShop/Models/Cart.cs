using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        // Active means you can add more cart items, otherwise you should create a new cart
        public bool Active { get; set; } = true;
        public virtual User User { get; set; }
    }
}
