using System.ComponentModel.DataAnnotations;

namespace app.Models.AdminViewModels
{
    public class CreateManyUsersViewModel
    {
        [Required]
        [EmailAddressArray(ErrorMessage = "Email address is not valid")]
        [Display(Name = "Epost")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}