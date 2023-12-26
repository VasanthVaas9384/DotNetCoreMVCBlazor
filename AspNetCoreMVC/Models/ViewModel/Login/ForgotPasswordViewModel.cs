using System.ComponentModel.DataAnnotations;

namespace AspNetCoreMVC.Models.ViewModel 
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email{ get; set; }
    }
}
