using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace app.Models.AdminViewModels
{
    public class CreateManyUsersViewModel
    {
        [Required]
        [EmailAddressArray(ErrorMessage = "{0} is not a valid email address.")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "The password must have at least one upper case letter, One lower case letter, One digit, One non alphanumeric character.")]
        public string Password { get; set; }
    }
}