using Ecommerce_api.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_api.ViewModels
{
    public class NewStoreViewModel
    {
        public string StoreName { get; set; }

        public IFormFile StoreLogo { get; set; }

        public IFormFile SignedContract { get; set; }

        public string StoreCode { get; set; }

        public StoreType StoreType { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public Province Province { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
