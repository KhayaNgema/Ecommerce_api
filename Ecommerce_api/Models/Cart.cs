using Ecommerce_api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_api.Models
{
    public class Cart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public UserBaseModel User { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        [ForeignKey("CreatedById")]
        public string CreatedById { get; set; }
        public virtual UserBaseModel CreatedBy { get; set; }


        [ForeignKey("ModifiedById")]
        public string ModifiedById { get; set; }
        public virtual UserBaseModel ModifiedBy { get; set; }

        public decimal CartTotal { get; set; }

        public List<CartItem> Items { get; set; }

        public Cart()
        {
            Items = new List<CartItem>();
        }

       /* public void AddItem(MenuItem menuItem, int quantity)
        {
            CartItem existingItem = Items.FirstOrDefault(item => item.MenuItem.MenuItemId == menuItem.MenuItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem { MenuItem = menuItem, Quantity = quantity });
            }
        }

        public void RemoveItem(int menuItemId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.MenuItem.MenuItemId == menuItemId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);

                System.Web.HttpContext.Current.Session["Cart"] = this;
            }
        }

        public void UpdateQuantity(int menuItemId, int quantity)
        {
            var itemToUpdate = Items.FirstOrDefault(item => item.MenuItem.MenuItemId == menuItemId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += (decimal)item.MenuItem.MenuItemPrice * item.Quantity;
            }
            return total;
        }*/
    }
}
