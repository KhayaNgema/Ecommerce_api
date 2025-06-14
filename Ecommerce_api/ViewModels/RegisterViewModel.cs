using System.ComponentModel.DataAnnotations;

namespace Ecommerce_api.ViewModels
{
    public class RegisterViewModel
    {
        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string PhoneNumber { get; set; }

        public string StreetNumber { get; set; }

        public string Surbub { get; set; }

        public string City_town { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public string Zip_code { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        [StringLength(100, ErrorMessage = "The password must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
