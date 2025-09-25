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
        public async Task<IActionResult> GetProducts()
        {
            var items = await _context.Products
                .Include(i => i.Category)
                .Include(i => i.CreatedBy)
                .Include(i => i.ModifiedBy)
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
                category = new
                {
                    categoryName = item.Category.CategoryName,
                }
            });


            return Ok(result);
        }


        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
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

            return Ok(categories.Select(c => new
            {
                CategoryName = c.Name,
                CategoryId = _encryptionService.Encrypt(c.CategoryId)
            }));
        }

        [Authorize(Roles ="System Administrator")]
        [HttpPost("new_product")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductViewModel viewModel)
        {
            var user = User;

            return await _productService.CreateProductAsync(viewModel, User);
        }

        [Authorize(Roles = "System Administrator")]
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

                return Ok(new
                {
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

        [Authorize(Roles = "System Administrator")]
        [HttpGet("get_product")]
        public async Task<IActionResult> GetProduct([FromQuery] string productId)
        {
            var decryptedProductId = _encryptionService.DecryptToInt(productId);

            var product = await _context.Products.FindAsync(decryptedProductId);

            if (product == null)
            {
                return NotFound(new { success = false, message = "Product not found." });
            }

            var viewModel = new UpdateProductViewModel
            {
                ProductId = productId,
                ProductName = product.ProductName,
                Description = product.Description,
                SellingPrice = product.SellingPrice,
                CostPrice = product.CostPrice,
                Barcode = product.Barcode,
                Size = product.Size,
                ProductImageUrl = product.ProductImage 
            };

            return Ok(viewModel);
        }

        [Authorize(Roles = "System Administrator")]
        [HttpPut("update_product")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductViewModel viewModel, [FromQuery] string productId)
        {
            {
                try
                {
                    var decryptedProductId = _encryptionService.DecryptToInt(productId);

                    var product = await _context.Products.FindAsync(decryptedProductId);
                    if (product == null)
                        return NotFound(new { success = false, message = "Product not found" });

                    product.ProductName = viewModel.ProductName;
                    product.Description = viewModel.Description;
                    if (viewModel.SellingPrice.HasValue)
                    {
                        product.SellingPrice = viewModel.SellingPrice.Value;
                    }

                    if (viewModel.CostPrice.HasValue)
                    {
                        product.CostPrice = viewModel.CostPrice.Value;
                    }
                    product.Barcode = viewModel.Barcode;
                    product.Size = viewModel.Size;

                    if (viewModel.ProductImages != null && viewModel.ProductImages.Length > 0)
                    {
                        var uploadedPaths = await _fileUploadService.UploadFileAsync(viewModel.ProductImages);
                        product.ProductImage = string.Join(',', uploadedPaths);
                    }
                    else if (!string.IsNullOrEmpty(viewModel.ProductImageUrl))
                    {
                        product.ProductImage = viewModel.ProductImageUrl;
                    }

                    await _context.SaveChangesAsync();

                    return Ok(new { success = true, message = $"{viewModel.ProductName} details updated" });

                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                    var stackTrace = ex.StackTrace ?? "No stack trace available";

                    return StatusCode(500, new
                    {
                        success = false,
                        message = "An error occurred while updating the product.",
                        details = ex.Message,
                        innerDetails = innerMessage,
                        stackTrace = stackTrace
                    });
                }

            }
        }

        [Authorize(Roles = "System Administrator")]
        [HttpDelete("delete_product")]
        public async Task<IActionResult> DeleteProduct([FromQuery] string productId)
        {
            try
            {
                var decryptedProductId = _encryptionService.DecryptToInt(productId);

                var product = await _context.Products.FindAsync(decryptedProductId);

                if (product == null)
                    return NotFound(new { success = false, message = "Product not found." });

                _context.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Product deleted successfully." });
            }

            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                var stackTrace = ex.StackTrace ?? "No stack trace available";

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while updating the product.",
                    details = ex.Message,
                    innerDetails = innerMessage,
                    stackTrace = stackTrace
                });
            }
        }
    }

}

