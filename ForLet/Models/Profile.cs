using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ProfileLocation { get; set; }
        [ForeignKey("ClientId")]
        public AppUser Client { get; set; }
    }
}
