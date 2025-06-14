using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_api.Models
{
    public class OrderItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
    }
}
