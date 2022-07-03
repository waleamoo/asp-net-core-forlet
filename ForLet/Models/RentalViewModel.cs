using ForLet.Infrastructure;
using ForLet.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class RentalViewModel
    {
        public int Id { get; set; }

        // [Required, MinLength(2, ErrorMessage = "Mininum length is 2")]
        [MinLength(5, ErrorMessage = "Please supply a descriptive title."), MaxLength(100, ErrorMessage = "Title too long")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Select the property type")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a property type")]
        public int PropertyTypeId { get; set; } // type of property to be rented 

        [Required(ErrorMessage = "Rental description is required"), MinLength(5, ErrorMessage = "Please supply a descriptive rental description.")]
        public string RentalDescription { get; set; } //property type, address, 

        [Required(ErrorMessage = "Rental address is required"), MinLength(5, ErrorMessage = "Please supply a descriptive address.")]
        public string RentalAddress { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Number of occupants/apartments must be numeric")]
        [Required(ErrorMessage = "Number of apartment is required"), MaxLength(3, ErrorMessage = "Number of occupants/apartments cannot be more than 3 digit")]
        public string NoOfApartment { get; set; }

        [Required(ErrorMessage = "Rent amount is required"), Column(TypeName = "decimal(18,2)")]
        public decimal Rent { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Agreement amount must be numeric")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Agreement { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Damages amount must be numeric")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Damages { get; set; }

        [DataType(DataType.Date)]
        public DateTime DatePosted { get; set; }
        
        public string UserId { get; set; } // the userIds are weird characters 

        public string slug { get; set; }
        [ForeignKey("PropertyTypeId")]
        public virtual PropertyType propertyType { get; set; }

        [Required(ErrorMessage = "Please select a state")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a state")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "Please select a L.G.A")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a L.G.A")]
        public int LgaId { get; set; }

        public bool IsLive { get; set; }

        //[NotMapped]
        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        //[NotMapped]
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(50, ErrorMessage = "Name too long")]
        public string FullName { get; set; }

        //[NotMapped]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(50, ErrorMessage = "Email too long")]
        public string Email { get; set; }

        //[NotMapped]
        [Required(ErrorMessage = "House number is required")]
        [StringLength(5)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "House number must be numeric")]
        public string HouseNumber { get; set; }

        //[NotMapped]
        [Required(ErrorMessage = "Street Name is required")]
        [StringLength(50)]
        public string StreetName { get; set; }

        //[NotMapped]
        [Required(ErrorMessage = "Area Name is required")]
        [StringLength(50)]
        public string AreaName { get; set; }

        //[NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }

        public ICollection<Image> image { get; set; }

        public List<IFormFile> GalleryFiles { get; set; }
    }
}
