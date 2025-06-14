using Ecommerce.Services;
using Ecommerce_api.Data;
using Ecommerce_api.Models;
using Ecommerce_api.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce_api.Services
{
    public class ProductService : IProductService
    {
        private readonly Ecommerce_apiDBContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly FileUploadService _fileUploadService;
        private readonly RequestLogService _requestLogService;

        public ProductService(Ecommerce_apiDBContext context, UserManager<UserBaseModel> userManager,
            FileUploadService fileUploadService, RequestLogService requestLogService)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _requestLogService = requestLogService;
        }

        public async Task<IActionResult> CreateProductAsync(ProductViewModel viewModel, ClaimsPrincipal user)
        {
            try
            {
                if (viewModel == null)
                    return new BadRequestObjectResult(new { success = false, message = "Product data is missing." });

                var authenticatedUser = await _userManager.GetUserAsync(user);
                if (authenticatedUser == null)
                    return new UnauthorizedObjectResult(new { success = false, message = "User is not authenticated." });

                var product = new Product
                {
                    ProductName = viewModel.ProductName,
                    Description = viewModel.Description,
                    Barcode = viewModel.Barcode,
                    CostPrice = viewModel.CostPrice,
                    SellingPrice = viewModel.SellingPrice,
                    IsSelected = false,
                    InStock = 0,
                    Size = viewModel.Size,
                    Availability = Availability.Unavailable,
                    CreatedById = authenticatedUser.Id,
                    ModifiedById = authenticatedUser.Id,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    CategoryId = viewModel.CategoryId
                };

                if (viewModel.ProductImages != null && viewModel.ProductImages.Length > 0)
                {
                    var uploadedPath = await _fileUploadService.UploadFileAsync(viewModel.ProductImages);
                    product.ProductImage = uploadedPath;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return new CreatedAtActionResult("GetInventory", "ProductsApi", new { id = product.ProductId }, new
                {
                    success = true,
                    message = "Product created successfully.",
                    productName = product.ProductName
                });
            }
            catch (Exception ex)
            {
                await _requestLogService.LogFailedRequest("Failed to create a new product", StatusCodes.Status500InternalServerError);
                return new ObjectResult(new
                {
                    success = false,
                    message = "Failed to create a new product: " + ex.Message,
                    errorDetails = new
                    {
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    }
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

    }
}
