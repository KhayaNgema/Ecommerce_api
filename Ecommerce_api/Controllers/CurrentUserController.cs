using Ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentUserController : ControllerBase
    {
        private readonly UserManager<UserBaseModel> _userManager;

        public CurrentUserController(UserManager<UserBaseModel> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("current_user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            var response = new Dictionary<string, object>
            {
                ["Id"] = user.Id,
                ["FirstName"] = user.FirstName,
                ["LastName"] = user.LastName,
                ["Email"] = user.Email,
                ["PhoneNumber"] = user.PhoneNumber,
                ["DateOfBirth"] = user.DateOfBirth,
                ["ProfilePicture"] = user.ProfilePicture,
                ["Address"] = user.Address,
                ["IsActive"] = user.IsActive,
                ["IsSuspended"] = user.IsSuspended,
                ["IsDeleted"] = user.IsDeleted,
                ["IsFirstTimeLogin"] = user.IsFirstTimeLogin,
                ["CreatedDateTime"] = user.CreatedDateTime,
                ["ModifiedDateTime"] = user.ModifiedDateTime,
                ["CreatedBy"] = user.CreatedBy,
                ["ModifiedBy"] = user.ModifiedBy,
                ["Roles"] = roles
            };

            switch (user)
            {
                case Customer customer:
                    response["AccountNumber"] = customer.AccountNumber;
                    break;

                case SalesAssociate associate:
                    response["AssociateNumber"] = associate.AssociateNumber;
                    response["StoreId"] = associate.StoreId;
                    break;

                case StoreManager manager:
                    response["StoreId"] = manager.StoreId;
                    break;
            }

            return Ok(response);
        }
    }
}
