using AspNetCoreMVC.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreMVC.Models.ViewModel 
{
    public class CreateViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }
        [Display(Name = "Office Email")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
            ErrorMessage = "Invalid email format")]
        [Required]
        public string Email { get; set; }
        [Required]
        public Department? Department { get; set; }

        public List<IFormFile>? Photos { get; set; }
    }
}
