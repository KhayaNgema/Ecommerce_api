using System.Text;
using System.Text.Encodings.Web;
using Ecommerce.Services;
using Ecommerce_api.Data;
using Ecommerce_api.Interfaces;
using Ecommerce_api.Models;
using Ecommerce_api.Services;
using Ecommerce_api.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IUserStore<UserBaseModel> _userStore;
        private readonly IUserEmailStore<UserBaseModel> _emailStore;
        private readonly ILogger<RegisterController> _logger;
        private readonly FileUploadService _fileUploadService;
        private readonly EmailService _emailService;
        private readonly Ecommerce_apiDBContext _context;
        private readonly RequestLogService _requestLogService;
        private readonly EncryptionService _encryptionService;

        public RegisterController(
            UserManager<UserBaseModel> userManager,
            IUserStore<UserBaseModel> userStore,
            ILogger<RegisterController> logger,
            FileUploadService fileUploadService,
            EmailService emailService,
            Ecommerce_apiDBContext context,
            RequestLogService requestLogService,
             EncryptionService encryptionService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _fileUploadService = fileUploadService;
            _emailService = emailService;
            _context = context;
            _requestLogService = requestLogService;
            _encryptionService = encryptionService;
        }

        [HttpPost("create_new_account")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel viewModel)
        {
/*            if (!viewModel.AcceptTerms)
            {
                await _requestLogService.LogFailedRequest("Terms not accepted", StatusCodes.Status400BadRequest);
                return BadRequest("You must accept our terms and conditions to continue.");
            }*/

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUserByPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == viewModel.PhoneNumber);

            if (existingUserByPhone != null)
            {
                await _requestLogService.LogFailedRequest("Phone number exists", StatusCodes.Status400BadRequest);
                return Conflict("An account with this phone number already exists.");
            }

            var existingUserByEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);
            if (existingUserByEmail != null)
            {
                await _requestLogService.LogFailedRequest("Email exists", StatusCodes.Status400BadRequest);
                return Conflict("An account with this email already exists.");
            }

            var user = new Customer
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                DateOfBirth = viewModel.DateOfBirth,
                IsActive = true,
                IsSuspended = false,
                Address = $"{viewModel.StreetNumber}, {viewModel.Surbub}, {viewModel.City_town}, {viewModel.Province}, {viewModel.Country}, {viewModel.Zip_code}"
            };

            if (viewModel.ProfilePicture != null && viewModel.ProfilePicture.Length > 0)
            {
                user.ProfilePicture = await _fileUploadService.UploadFileAsync(viewModel.ProfilePicture);
            }

            await _userStore.SetUserNameAsync(user, viewModel.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, viewModel.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await _requestLogService.LogFailedRequest("User creation failed", StatusCodes.Status400BadRequest);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("User created a new account with password.");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = $"https://yourdomain.com/confirm-email?userId={userId}&code={code}";
            var emailBody = $"Hello {user.FirstName},<br><br>" +
                            $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(user.Email, "Confirm your email", emailBody, "Diski 360"));

            // If you have subscription/agreement models to persist, uncomment and update accordingly:
            // _context.Add(subscription);
            // _context.Add(agreement);

            await _context.SaveChangesAsync();

            await _requestLogService.LogSuceededRequest("Account created successfully.", StatusCodes.Status200OK);

            return Ok(new { message = "Registration successful. Please confirm your email." });
        }

        private IUserEmailStore<UserBaseModel> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The user store does not support email.");
            return (IUserEmailStore<UserBaseModel>)_userStore;
        }
    }
}
