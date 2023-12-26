using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreMVC.Models.ViewModel 
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Rememeber Me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl{ get; set; }

        public IList<AuthenticationScheme> ExternalIdentityLogins { get; set; }

    }
}
