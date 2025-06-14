using System.ComponentModel.DataAnnotations;

namespace Ecommerce_api.ViewModels
{
    public class ProductViewModel
    {
        public string ProductName { get; set; }
        public string Description { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal CostPrice { get; set; }

        public string Barcode { get; set; }

        public string Size { get; set; }

        public int CategoryId { get; set; }

        public IFormFile ProductImages { get; set; }
    }
}
