using System.ComponentModel.DataAnnotations;

namespace AspNetCoreMVC.Models.ViewModel 
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email{ get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password{ get; set; }

         
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password and confirm passowrd must be same.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
