using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ForLet.Models
{
    public class User
    {
        public string Id { get; set; }
        // columns to be added/filled at registration + AppUser Columns
        public string UserName { get; set; }

        [Required, MinLength(2, ErrorMessage = "Mininum length is 2")]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters")]
        public string FirstName { get; set; }
        [Required, MinLength(2, ErrorMessage = "Mininum length is 2")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address (Optional)")]
        public string Email { get; set; }
        
        [Required, StringLength(11, ErrorMessage = "Phone number must be 11 digit")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Mininum password length is 4")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        public string Password { get; set; }

        [NotMapped]
        [Required]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "House number is required")]
        [StringLength(5)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "House number must be numeric")]
        public string HouseNumber { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Street Name is required")]
        [StringLength(50)]
        public string StreetName { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Area Name is required")]
        [StringLength(50)]
        public string AreaName { get; set; }

        public ICollection<Rental> Rentals { get; set; }
        //public ICollection<Profile> Profile { get; set; } 
    }
}
