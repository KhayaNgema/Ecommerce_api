using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using Ecommerce_api.Models;


namespace Ecommerce_api.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        [ForeignKey("CreatedById")]
        public string CreatedById { get; set; }
        public virtual UserBaseModel CreatedBy { get; set; }


        [ForeignKey("ModifiedById")]
        public string ModifiedById { get; set; }
        public virtual UserBaseModel ModifiedBy { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public bool Deleted { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
