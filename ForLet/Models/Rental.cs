using ForLet.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class Rental
    {
        public int Id { get; set; }

        public string Title { get; set; } 

        public string RentalDescription { get; set; } 

        public string RentalAddress { get; set; }

        public string NoOfApartment { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Rent amount must be numeric")]
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Rent { get; set; }

        [RegularExpression(@"^[1-9]+$", ErrorMessage = "Agreement amount must be numeric")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Agreement { get; set; }

        [RegularExpression(@"^[1-9]+$", ErrorMessage = "Damages amount must be numeric")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Damages { get; set; }

        public bool IsLive { get; set; }

        [DataType(DataType.Date)]
        public DateTime DatePosted { get; set; }

        public string ClientId { get; set; }
        [ForeignKey("ClientId")]
        public AppUser Client { get; set; }

        public string slug { get; set; }

        public int? PropertyTypeId { get; set; }
        [ForeignKey("PropertyTypeId")]
        public PropertyType propertyType { get; set; }

        public int? StateId { get; set; }
        [ForeignKey("StateId")]
        public State state { get; set; }

        public int? LgaId { get; set; }
        [ForeignKey("LgaId")]
        public LGA lga { get; set; }

        public List<Image> image { get; set; }
        //public ICollection<Image> image { get; set; }
        
    }
}
