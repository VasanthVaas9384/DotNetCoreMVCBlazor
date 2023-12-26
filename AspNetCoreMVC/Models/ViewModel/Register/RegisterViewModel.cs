using AspNetCoreMVC.Utilities.CustomValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreMVC.Models.ViewModel 
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "account")]
        [ValidEmailDomain(allowedDomain: "gmail.com", ErrorMessage = $"Domain must be gmail.com")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password must be same")]
        public string ConfirmPassword { get; set; }

        public string City { get; set; }


    }
}
