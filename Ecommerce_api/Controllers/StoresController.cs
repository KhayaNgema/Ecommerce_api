using Ecommerce.Services;
using Ecommerce_api.Data;
using Ecommerce_api.Models;
using Ecommerce_api.Services;
using Ecommerce_api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : Controller
    {
        private readonly Ecommerce_apiDBContext _context;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly FileUploadService _fileUploadService;
        private readonly IProductService _productService;
        private readonly RequestLogService _requestLogService;
        private readonly EncryptionService _encryptionService;


        public StoresController(Ecommerce_apiDBContext context, UserManager<UserBaseModel> userManager,
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

        [Authorize]
        [HttpGet("stores")]
        public async Task<IActionResult> Stores()
        {
            var stores = await _context.Stores
                .Where(s => !s.IsDeleted)
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .ToListAsync();

            var result = stores.Select(s => new
            {
                s.StoreId,
                s.StoreName,
                s.StoreLogo,
                s.StoreCode,
                s.StoreType,
                s.IsActive,
                s.HasPaid,
                s.CreatedDateTime,
                s.ModifiedDateTime,

                CreatedBy = s.CreatedBy == null ? null : new
                {
                    s.CreatedBy.Id,
                    FullName = s.CreatedBy.FirstName + " " + s.CreatedBy.LastName,
                    s.CreatedBy.Email
                },

                ModifiedBy = s.ModifiedBy == null ? null : new
                {
                    s.ModifiedBy.Id,
                    FullName = s.ModifiedBy.FirstName + " " + s.ModifiedBy.LastName,
                    s.ModifiedBy.Email
                }
            });

            return Ok(result);
        }


        [Authorize]
        [HttpPost("newStore")]
        public async Task<IActionResult> CreateStore([FromForm] NewStoreViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);

            var store = new Store
            {
                StoreName = viewModel.StoreName,
                StoreCode = viewModel.StoreCode,
                StoreType = viewModel.StoreType,
                IsSuspended = false,
                IsDeleted = false,
                IsActive = true,
                HasPaid = false,
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                CreatedById = user.Id,
                ModifiedById = user.Id,
                Address = $"{viewModel.Street}, {viewModel.City}, {viewModel.PostalCode}, {viewModel.Province}, {viewModel.Country}",
                Longitude = viewModel.Longitude,
                Latitude = viewModel.Latitude
            };

            if (viewModel.StoreLogo != null && viewModel.StoreLogo.Length > 0)
            {
                var logoPath = await _fileUploadService.UploadFileAsync(viewModel.StoreLogo);
                store.StoreLogo = logoPath;
            }

            if (viewModel.SignedContract != null && viewModel.SignedContract.Length > 0)
            {
                var contractPath = await _fileUploadService.UploadFileAsync(viewModel.SignedContract);
                store.SignedContract = contractPath;
            }

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return Ok(store);
        }

    }
}
