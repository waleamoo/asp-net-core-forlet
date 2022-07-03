﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class PropertyType
    {
        [Key]
        public int Id { get; set; }
        public string PropertyName { get; set; }
    }
}
