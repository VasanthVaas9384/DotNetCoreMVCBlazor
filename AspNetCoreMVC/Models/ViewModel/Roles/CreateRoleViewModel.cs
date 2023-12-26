using System.ComponentModel.DataAnnotations;

namespace AspNetCoreMVC.Models.ViewModel 
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
