using Ecommerce_api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Services;
using Ecommerce_api.Data;
using Ecommerce_api.Models;
using Ecommerce_api.Services;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_api.ViewModels;
using System.Web;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly Ecommerce_apiDBContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly FileUploadService _fileUploadService;
        private readonly IProductService _productService;
        private readonly RequestLogService _requestLogService;
        private readonly EncryptionService _encryptionService;


        public ProductsController(Ecommerce_apiDBContext context, UserManager<UserBaseModel> userManager,
            FileUploadService fileUploadService, RequestLogService requestLogService, IProductService productService,
            EncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _requestLogService = requestLogService;
            _productService = productService;
            _encryptionService = encryptionService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(string storeId)
        {
            var decodedStoreId = HttpUtility.UrlDecode(storeId);
            var decryptedStoreId = _encryptionService.DecryptToInt(decodedStoreId);

            var items = await _context.Products
                .Include(i => i.Category)
                .Include(i => i.CreatedBy)
                .Include(i => i.ModifiedBy)
                .Where(i => i.StoreId == decryptedStoreId)
                .OrderByDescending(i => i.CreatedDateTime)
                .ToListAsync();

            if (items == null || !items.Any())
            {
                return NotFound(new { success = false, message = "No products found." });
            }

            var result = items.Select(item => new
            {
                productId = _encryptionService.Encrypt(item.ProductId),
                productName = item.ProductName,
                description = item.Description,
                costPrice = item.CostPrice,
                sellingPrice = item.SellingPrice,
                categoryId = item.CategoryId,
                productImage = item.ProductImage,
                size = item.Size,
                barcode = item.Barcode,
                createdBy = new
                {
                    userId = _encryptionService.Encrypt(item.CreatedBy.Id),
                    firstName = item.CreatedBy.FirstName,
                    lastName = item.CreatedBy.LastName
                },
                modifiedBy = new
                {
                    userId = _encryptionService.Encrypt(item.ModifiedBy.Id),
                    firstName = item.ModifiedBy.FirstName,
                    lastName = item.ModifiedBy.LastName
                },
                category = new
                {
                    categoryId = _encryptionService.Encrypt(item.Category.CategoryId),
                    categoryName = item.Category.CategoryName,
                }
            });


            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(string storeId)
        {
            var decryptedStoreId = _encryptionService.DecryptToInt(storeId);

            var categories = await _context.Categories
                .Where(c => c.StoreId == decryptedStoreId)
                .Select(t => new
                {
                    CategoryId = t.CategoryId.ToString(),
                    Name = t.CategoryName.Replace("_", " ")
                })
                .ToListAsync();

            if (categories == null || !categories.Any())
            {
                return NotFound(new { success = false, message = "No categories found." });
            }

            return Ok(categories.Select(c => new { 
                CategoryName = c.Name,
                CategoryId = _encryptionService.Encrypt(c.CategoryId)
            }));
        }

        [HttpPost("new_product")]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] ProductViewModel viewModel, [FromQuery] string storeId)
        {
            var user = User;

            return await _productService.CreateProductAsync(viewModel, user, storeId);
        }

        [HttpPost("new_category")]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryViewModel viewModel)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                    return Unauthorized(new { success = false, message = "User is not authenticated." });

                var category = new Category
                {
                    CategoryName = viewModel.CategoryName,
                    CreatedById = user.Id,
                    CreatedDateTime = DateTime.Now,
                    ModifiedById = user.Id,
                    ModifiedDateTime = DateTime.Now,
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Category created successfully.", 
                    category = category.CategoryName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}
