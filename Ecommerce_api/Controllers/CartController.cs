using Ecommerce_api.Data;
using Ecommerce_api.Models;
using Ecommerce_api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly Ecommerce_apiDBContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly EncryptionService _encryptionService;

        public CartController(Ecommerce_apiDBContext context, 
            UserManager<UserBaseModel> userManager,
            EncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
        }

        [HttpPost("add_to_cart")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] CartItemViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = user.Id,
                    CreatedById = user.Id,
                    ModifiedById = user.Id,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    CartTotal = 0m,
                    Items = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var decryptedProductId = _encryptionService.DecryptToInt(vm.ProductId);

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == decryptedProductId && !i.Deleted);

            var product = await _context.Products.FindAsync(decryptedProductId);

            if (product == null)
                return BadRequest(new { success = false, message = "Product not found or unavailable." });

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = decryptedProductId,
                    Quantity = vm.Quantity,
                    CreatedById = user.Id,
                    ModifiedById = user.Id,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    Deleted = false,
                    Subtotal = product.SellingPrice * vm.Quantity,
                    Cart = cart
                };
                cart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += vm.Quantity;
                cartItem.ModifiedById = user.Id;
                cartItem.ModifiedDateTime = DateTime.UtcNow;
                cartItem.Subtotal = product.SellingPrice * cartItem.Quantity;
            }

            cart.CartTotal = cart.Items.Where(i => !i.Deleted).Sum(i => i.Subtotal);
            cart.ModifiedById = user.Id;
            cart.ModifiedDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = $"{vm.Quantity} {product.ProductName}(s) added to cart." });
        }

        [HttpGet("get_cat_items")]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items.Where(i => !i.Deleted))
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
                return Ok(new CartViewModel { Items = new List<CartItemDisplayViewModel>(), CartTotal = 0m });

            var vm = new CartViewModel
            {
                Items = cart.Items.Select(i => new CartItemDisplayViewModel
                {
                    CartItemId = i.CartItemId,
                    ProductId = i.ProductId,
                    ProductName = i.Product.ProductName,
                    SellingPrice = i.Product.SellingPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                }).ToList(),
                CartTotal = cart.CartTotal
            };

            return Ok(vm);
        }

        [HttpPost("clear_cart")]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
                return Ok(new { success = true, message = "Cart already empty." });

            foreach (var item in cart.Items)
            {
                item.Deleted = true;
                item.ModifiedById = user.Id;
                item.ModifiedDateTime = DateTime.UtcNow;
            }

            cart.CartTotal = 0m;
            cart.ModifiedById = user.Id;
            cart.ModifiedDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Cart cleared." });
        }
    }
}
