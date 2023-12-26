using Microsoft.AspNetCore.Identity;

namespace AspNetCoreMVC.Models.AppDBContext.ExtendIdentityUser
{
    public class ApplicationUser : IdentityUser
    {
        public string city { get; set; }
    }
}
