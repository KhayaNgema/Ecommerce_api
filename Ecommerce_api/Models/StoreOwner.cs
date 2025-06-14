namespace Ecommerce_api.Models
{
    public class StoreOwner : UserBaseModel
    {
        public virtual ICollection<Store> Stores { get; set; }
    }
}
