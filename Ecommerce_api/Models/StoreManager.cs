using Ecommerce_api.Models;

namespace Ecommerce_api.Models
{
    public class StoreManager : UserBaseModel
    {
        public string AssociateNumber { get; set; }

        public int StoreId { get; set; }

        public virtual Store Store { get; set; }
    }
}
