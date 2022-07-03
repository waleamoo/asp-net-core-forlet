using ForLet.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class AppUser : IdentityUser
    {
        // addtional columns 
        [Required, MinLength(2, ErrorMessage = "Mininum length is 2")]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters")]
        public string FirstName { get; set; }
        [Required, MinLength(2, ErrorMessage = "Mininum length is 2")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }

        // these fields are for update for User Information 
        [NotMapped]
        [FileExtension]
        public IFormFile ProfileImage { get; set; }

        [NotMapped]
        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Mininum password length is 4")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password), Required(ErrorMessage = "Please confirm your password"), MinLength(4, ErrorMessage = "Mininum password length is 4")]
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

        [NotMapped]
        [Required(ErrorMessage = "Please select a state")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a state")]
        public int StateId { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select a L.G.A")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a L.G.A")]
        public int LgaId { get; set; }

    }
}
