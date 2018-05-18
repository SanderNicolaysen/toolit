using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace app.Models.AdminViewModels
{
    public class EmailAddressArrayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid (Object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(this.ErrorMessage);
            
            string e = value as string;
            string[] emails = e.Split( new[] { Environment.NewLine }, StringSplitOptions.None );
            for (int i = 0; i < emails.Length; i++) 
            {
                if (!IsValidEmail(emails[i]))            
                    return new ValidationResult(this.ErrorMessage);
            }
            return ValidationResult.Success;
        }

        private bool IsValidEmail(string email)
        {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }
    }
}