using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class Login
    {

        [Required, StringLength(11, ErrorMessage = "Phone number must be 11 digit")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Password mininum length is 4")]
        public string Password { get; set; }
        // this is the url generated when a user tries to access a page that need authorization
        public string ReturnUrl { get; set; }  
    }
}
