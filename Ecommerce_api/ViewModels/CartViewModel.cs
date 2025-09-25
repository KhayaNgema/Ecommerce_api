namespace Ecommerce_api.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemDisplayViewModel> Items { get; set; }
        public decimal CartTotal { get; set; }
    }

}
