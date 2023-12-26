using System.ComponentModel.DataAnnotations;

namespace AspNetCoreMVC.Utilities.CustomValidation
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowedDomain;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }

        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                string userDomain = value.ToString().Split('@')[1].ToString();

                if (userDomain.ToUpper() == allowedDomain.ToUpper())
                    return true;
            }
            return false;
        }
    }
}
