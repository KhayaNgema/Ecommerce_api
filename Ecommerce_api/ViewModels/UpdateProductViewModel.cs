using Ecommerce_api.Models;

namespace Ecommerce_api.ViewModels
{
    public class UpdateProductViewModel
    {
        public string? ProductId { get; set; }

        public string? ProductName { get; set; }
        public string? Description { get; set; }

        public decimal? SellingPrice { get; set; }

        public decimal? CostPrice { get; set; }

        public string? Barcode { get; set; }

        public string? Size { get; set; }

        public string? ProductImageUrl { get; set; }

        public IFormFile? ProductImages { get; set; }
    }
}
