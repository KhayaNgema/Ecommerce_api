using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_api.Models
{
    public class StoreOwnerStore
    {
        public string StoreOwnerId { get; set; }
        [ForeignKey("StoreOwnerId")]
        public StoreOwner StoreOwner { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }
    }
}
