using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_api.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Menu item Id")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Item name")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Required]
        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        [Display(Name = "Item image(s)")]
        public string ProductImage { get; set; }

        [Display(Name = "Is selected")]
        public bool IsSelected { get; set; }

        [Display(Name = "Size")]
        public string? Size { get; set; }

        public string Barcode { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        [ForeignKey("CreatedById")]
        public string CreatedById { get; set; }
        public virtual UserBaseModel CreatedBy { get; set; }


        [ForeignKey("ModifiedById")]
        public string ModifiedById { get; set; }
        public virtual UserBaseModel ModifiedBy { get; set; }

        public Availability Availability { get; set; }

        public int InStock { get; set; }
    }
    public enum Availability
    {
        [Display(Name = "Available")]
        Available,

        [Display(Name = "Unavailable")]
        Unavailable
    }
}
