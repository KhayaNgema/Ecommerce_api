using Microsoft.AspNetCore.Identity;

namespace Ecommerce_api.Models
{
    public class UserBaseModel : IdentityUser
    {
        public UserBaseModel()
        {
            ProfilePicture = "Images/user_default_pic.jpg";
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? ProfilePicture { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsFirstTimeLogin { get; set; }

        public string Address { get; set; }
    }
}
