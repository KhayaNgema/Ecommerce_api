using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_api.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Display(Name = "Store")]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }

        [Display(Name = "Order number")]
        [ScaffoldColumn(false)]
        public string OrderNumber { get; set; }

        [Display(Name = "Order date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Total price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Status")]
        public Status Status { get; set; }

        [Display(Name = "Last updated at")]
        public DateTime LastUpdated { get; set; }

        [Display(Name = "Is Paid")]

        public string LastFourDigitsOfOrderNumber
        {
            get
            {
                return OrderNumber.Substring(OrderNumber.Length - 4);
            }
        }

        public bool IsPaid { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        [ForeignKey("CreatedById")]
        public string CreatedById { get; set; }
        public virtual UserBaseModel CreatedBy { get; set; }


        [ForeignKey("ModifiedById")]
        public string ModifiedById { get; set; }
        public virtual UserBaseModel ModifiedBy { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderDate = DateTime.Now;
        }
    }

    public enum Status
    {
        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Ready for collection")]
        Ready_For_Collection,

        [Display(Name = "Collected")]
        Collected,

        [Display(Name = "Cancelled")]
        Cancelled
    }
}
