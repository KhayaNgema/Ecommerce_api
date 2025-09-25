namespace Ecommerce_api.Models
{
    public class SalesAssociate : UserBaseModel
    {
        public string AssociateNumber { get; set; }

        public int StoreId { get; set; }
    }
}
