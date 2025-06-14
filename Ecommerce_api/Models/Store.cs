using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_api.Models
{
    public class Store : Location
    {
        public int StoreId { get; set; }

        public string StoreName { get; set; }

        public string StoreLogo { get; set; }

        public string StoreCode { get; set; }

        public StoreType StoreType { get; set; }

        public string? SignedContract { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }
        public bool HasPaid { get; set; }

        public Store()
        {
            StoreLogo = "Images/store_logo.jpeg";
        }

        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        [ForeignKey("CreatedById")]
        public string CreatedById { get; set; }
        public virtual UserBaseModel CreatedBy { get; set; }


        [ForeignKey("ModifiedById")]
        public string ModifiedById { get; set; }
        public virtual UserBaseModel ModifiedBy { get; set; }
    }

    public enum StoreType
    {
        [Display(Name = "Apparel & Accessories")]
        Apparel_And_Accessories,

        [Display(Name = "Home & Living")]
        Home_And_Living,

        [Display(Name = "Health & Beauty")]
        Health_And_Beauty,

        [Display(Name = "Food & Beverage")]
        Food_And_Beverage,

        [Display(Name = "Entertainment & Leisure")]
        Entertainment_And_Leisure,

        [Display(Name = "Sports & Outdoors")]
        Sports_And_Outdoors,

        [Display(Name = "Electronics & Appliances")]
        Electronics_And_Appliances,

        [Display(Name = "Automotive")]
        Automotive,

        [Display(Name = "Hardware & DIY")]
        Hardware_And_DIY,

        [Display(Name = "Baby & Kids")]
        Baby_And_Kids,

        [Display(Name = "Pet Supplies")]
        Pet_Supplies,

        [Display(Name = "Office & Stationery")]
        Office_And_Stationery
    }
}
