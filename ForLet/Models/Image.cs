using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public int RentalId { get; set; }
        public string ImageLocation { get; set; }
        public Rental rental { get; set; }
    }
}
