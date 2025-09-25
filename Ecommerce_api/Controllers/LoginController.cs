using Ecommerce_api.Models;
using Ecommerce_api.Services;
using Ecommerce_api.Helpers;
using Ecommerce_api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<UserBaseModel> _signInManager;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IActivityLogger _activityLogger;
        private readonly IConfiguration _configuration;
        private readonly EncryptionService _encryptionService;

        public AuthController(
            SignInManager<UserBaseModel> signInManager,
            UserManager<UserBaseModel> userManager,
            ILogger<AuthController> logger,
            IActivityLogger activityLogger,
            IConfiguration configuration,
            EncryptionService encryptionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _activityLogger = activityLogger;
            _configuration = configuration;
            _encryptionService = encryptionService;
        }

        public class LoginRequest
        {
            [Required(ErrorMessage = "The email or phone field is required.")]
            public string EmailOrPhone { get; set; }

            [Required(ErrorMessage ="The password field is required")]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { message = "Validation failed", errors });
            }

            var isEmail = new EmailAddressAttribute().IsValid(request.EmailOrPhone);
            var user = isEmail
                ? await _userManager.FindByEmailAsync(request.EmailOrPhone)
                : await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.EmailOrPhone);

            if (user == null)
                return NotFound("User account does not exist.");

            if (user.IsDeleted || user.IsSuspended || !user.IsActive || !user.EmailConfirmed)
                return Unauthorized("Account is either deleted, suspended, or not confirmed.");

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, request.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
                return StatusCode(406, "Incorrect email or phone or password.");

            var roles = await _userManager.GetRolesAsync(user);
            string role = null;

            if (roles != null && roles.Any())
            {
                role = roles.FirstOrDefault();
            }
            else
            {
                role = "Customer";
            }


            var token = GenerateJwtToken(user, role);

            return Ok(new
            {
                Message = "Login successful",
                UserId = _encryptionService.Encrypt(user.Id),
                UserName = user.UserName,
                Email = user.Email,
                Role = role,
                Token = token
            });
        }


        private string GenerateJwtToken(UserBaseModel user, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
