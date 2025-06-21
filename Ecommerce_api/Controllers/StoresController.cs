using Ecommerce.Services;
using Ecommerce_api.Data;
using Ecommerce_api.Models;
using Ecommerce_api.Services;
using Ecommerce_api.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text;
using Ecommerce_api.DTOs;
using System.Web;

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
        private readonly IUserStore<UserBaseModel> _userStore;
        private readonly EmailService _emailService;
        private readonly RandomPasswordGeneratorService _passwordGenerator;
        private readonly IUserEmailStore<UserBaseModel> _emailStore;
        private readonly ILogger<StoresController> _logger;

        public StoresController(Ecommerce_apiDBContext context, 
            UserManager<UserBaseModel> userManager,
            FileUploadService fileUploadService,
            ILogger<StoresController> logger,
            RequestLogService requestLogService, 
            IProductService productService,
            EmailService emailService,
            RandomPasswordGeneratorService passwordGenerator,
            EncryptionService encryptionService,
            IUserStore<UserBaseModel> userStore)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _requestLogService = requestLogService;
            _productService = productService;
            _encryptionService = encryptionService;
            _userStore = userStore;
            _emailService = emailService;
            _emailStore = GetEmailStore();
            _passwordGenerator = passwordGenerator;
            _logger = logger;
        }


        [Authorize]
        [HttpGet("my_stores")]
        public async Task<IActionResult> MyStores()
        {
            var user = await _userManager.GetUserAsync(User);

            var myStoreIds = await _context.StoreOwnerStores
                .Where(sos => sos.StoreOwnerId == user.Id)
                .Select(sos => sos.StoreId)
                .ToListAsync();

            var stores = await _context.Stores
                .Where(s => !s.IsDeleted && myStoreIds.Contains(s.StoreId))
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .OrderByDescending(s => s.CreatedDateTime)
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
        [HttpGet("store_owners")]
        public async Task<IActionResult> StoreOwners()
        {
            var storeOwners = await _context.StoreOwners
                .Where(so => !so.IsDeleted)
                .OrderByDescending(so => so.CreatedDateTime)
                .ToListAsync();

            var result = storeOwners.Select(so => new
            {
                so.Id,
                so.FirstName,
                so.LastName,
                so.PhoneNumber,
                so.ProfilePicture,
                so.Email,
                so.IsActive,
                so.IsSuspended,
                so.CreatedDateTime,
                so.ModifiedDateTime,
                so.CreatedBy,
                so.ModifiedBy
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("stores")]
        public async Task<IActionResult> Stores()
        {
            var stores = await _context.Stores
                .Where(s => !s.IsDeleted)
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .OrderByDescending(s => s.CreatedDateTime)
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
        [HttpGet("store_details")]
        public async Task<IActionResult> StoreDetails(string storeId)
        {
            var decodedStoreId = HttpUtility.UrlDecode(storeId);
            var decryptedStoreId = _encryptionService.DecryptToInt(decodedStoreId);

            var store = await _context.Stores
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .FirstOrDefaultAsync(s => s.StoreId == decryptedStoreId);

            if (store == null)
                return NotFound("Store not found");

            var result = new
            {
                StoreId = store.StoreId,
                StoreName = store.StoreName,
                StoreLogo = store.StoreLogo,
                StoreCode = store.StoreCode,
                StoreType = store.StoreType,
                SignedContract = store.SignedContract,
                IsSuspended = store.IsSuspended,
                IsDeleted = store.IsDeleted,
                IsActive = store.IsActive,
                HasPaid = store.HasPaid,
                CreatedDateTime = store.CreatedDateTime,
                ModifiedDateTime = store.ModifiedDateTime,

                CreatedBy = store.CreatedBy == null ? null : new
                {
                    store.CreatedBy.Id,
                    FullName = store.CreatedBy.FirstName + " " + store.CreatedBy.LastName,
                    store.CreatedBy.Email
                },

                ModifiedBy = store.ModifiedBy == null ? null : new
                {
                    store.ModifiedBy.Id,
                    FullName = store.ModifiedBy.FirstName + " " + store.ModifiedBy.LastName,
                    store.ModifiedBy.Email
                }
            };

            return Ok(result);
        }




        [Authorize]
        [HttpPost("new_store")]
        [Consumes("multipart/form-data")]
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


        [Authorize]
        [HttpPost("new_store_owner")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> NewStoreOwner([FromForm] NewStoreOwnerViewModel viewModel)
        {
            try
            {
                var existingUserByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == viewModel.PhoneNumber);
                if (existingUserByPhoneNumber != null)
                {
                    ModelState.AddModelError("Input.PhoneNumber", "An account with this phone number already exists.");
                    return BadRequest(ModelState);
                }

                var existingUserByEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Input.Email", "An account with this email address already exists.");
                    return BadRequest(ModelState);
                }

                var currentUser = await _userManager.GetUserAsync(User);

                var storeOwner = new StoreOwner
                {
                    Title = viewModel.Title,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    DateOfBirth = viewModel.DateOfBirth,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    CreatedBy = $"{currentUser.FirstName} {currentUser.LastName}",
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = $"{currentUser.FirstName} {currentUser.LastName}",
                    ModifiedDateTime = DateTime.Now,
                    IsActive = true,
                    IsSuspended = false,
                    IsFirstTimeLogin = true,
                    AccessFailedCount = 0,
                    Address = string.Join(", ", new[] {
                viewModel.StreetNumber,
                viewModel.Surbub,
                viewModel.City_town,
                viewModel.Province.ToString(),
                viewModel.Zip_code,
                viewModel.Country
                }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    Gender = viewModel.Gender,
                    IsDeleted = false
                };

                if (viewModel.ProfilePicture != null && viewModel.ProfilePicture.Length > 0)
                {
                    var profilePicturePath = await _fileUploadService.UploadFileAsync(viewModel.ProfilePicture);
                    storeOwner.ProfilePicture = profilePicturePath;
                }

                await _userStore.SetUserNameAsync(storeOwner, viewModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(storeOwner, viewModel.Email, CancellationToken.None);

                string randomPassword = _passwordGenerator.GenerateRandomPassword();
                var result = await _userManager.CreateAsync(storeOwner, randomPassword);

                if (result.Succeeded)
                {
                    if (viewModel.Stores != null && viewModel.Stores.Any())
                    {
                        foreach (var storeId in viewModel.Stores)
                        {
                            _logger.LogInformation("Creating join for StoreOwnerId: {StoreOwnerId}, StoreId: {StoreId}", storeOwner.Id, storeId);

                            var joinEntity = new StoreOwnerStore
                            {
                                StoreOwnerId = storeOwner.Id,
                                StoreId = storeId
                            };

                            _context.Add(joinEntity);
                        }

                        await _context.SaveChangesAsync();
                    }


                    await _userManager.AddToRoleAsync(storeOwner, "Store Owner");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(storeOwner);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = storeOwner.Id, code = code },
                        protocol: Request.Scheme);

                    string accountCreationEmailBody = $"Hello {storeOwner.FirstName},<br><br>";
                    accountCreationEmailBody += $"Welcome to Ecommerce!<br><br>";
                    accountCreationEmailBody += $"You have been successfully added as Store Owner. Below are your login credentials:<br><br>";
                    accountCreationEmailBody += $"Email: {storeOwner.Email}<br>";
                    accountCreationEmailBody += $"Password: {randomPassword}<br><br>";
                    accountCreationEmailBody += $"Please note that we have sent you two emails, including this one. You need to open the other email to confirm your email address before you can log into the system.<br><br>";
                    accountCreationEmailBody += $"Thank you!";

                    BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(storeOwner.Email, "Welcome to Ecommerce", accountCreationEmailBody, "Ecommerce"));

                    string emailConfirmationEmailBody = $"Hello {storeOwner.FirstName},<br><br>";
                    emailConfirmationEmailBody += $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br><br>";
                    emailConfirmationEmailBody += $"Thank you!";

                    BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(storeOwner.Email, "Confirm Your Email Address", emailConfirmationEmailBody, "Ecommerce"));

                    var dto = new StoreOwnerDto
                    {
                        Id = storeOwner.Id,
                        FirstName = storeOwner.FirstName,
                        LastName = storeOwner.LastName,
                        Email = storeOwner.Email
                    };
                    return Ok(dto);

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Failed to onboard store owner: " + ex.Message,
                    errorDetails = new
                    {
                        innerException = ex.InnerException?.Message,
                        stackTrace = ex.StackTrace
                    }
                });
            }
        }


        private IUserEmailStore<UserBaseModel> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<UserBaseModel>)_userStore;
        }
    }
}
