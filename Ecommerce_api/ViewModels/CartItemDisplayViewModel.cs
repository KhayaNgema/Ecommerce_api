namespace Ecommerce_api.ViewModels
{
    public class CartItemDisplayViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

}
