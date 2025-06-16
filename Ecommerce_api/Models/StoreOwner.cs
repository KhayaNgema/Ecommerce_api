namespace Ecommerce_api.Models
{
    public class StoreOwner : UserBaseModel
    {
        public ICollection<StoreOwnerStore> StoreOwnerStores { get; set; } = new List<StoreOwnerStore>();
    }
}

