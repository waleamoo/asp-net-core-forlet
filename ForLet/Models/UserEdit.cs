using ForLet.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class UserEdit
    {
        public string Id { get; set; }
        [MinLength(2, ErrorMessage = "Mininum length is 2")]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters")]
        public string FirstName { get; set; }
        [Required, MinLength(2, ErrorMessage = "Mininum length is 2")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters")]
        public string LastName { get; set; }
        public string Address { get; set; }

        [FileExtension]
        public IFormFile Avatar { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address (Optional)")]
        public string Email { get; set; }

        [Required, StringLength(11, ErrorMessage = "Phone number must be 11 digit")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Password is required"), MinLength(4, ErrorMessage = "Mininum password length is 4")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        public string Password { get; set; }

        public UserEdit()
        {

        }

        public UserEdit(AppUser appUser)
        {
            FirstName = appUser.FirstName;
            LastName = appUser.LastName;
            Address = appUser.Address;
            PhoneNumber = appUser.PhoneNumber;
            Password = appUser.PasswordHash;
            Email = appUser.Email;
        }
    }
}
