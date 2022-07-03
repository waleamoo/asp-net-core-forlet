using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class LGA
    {
        [Key]
        public int Id { get; set; }
        public int StateId { get; set; } // foreign key 
        public string LGAName { get; set; }
        [ForeignKey("StateId")]
        public virtual State state { get; set; }

    }
}
