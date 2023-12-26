namespace AspNetCoreMVC.Models.ViewModel 
{
    public class ManageUserClaimsFromUserViewModel
    {
        public ManageUserClaimsFromUserViewModel()
        {
            Claims = new List<UserClaim>();
        }

        public string UserId{ get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
