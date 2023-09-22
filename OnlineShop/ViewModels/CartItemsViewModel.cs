namespace OnlineShop.ViewModels
{
    public class CartItemsViewModel
    {
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int InStock { get; set; }   
        public string Image { get; set; } = "Default.jpg";
    }
}
