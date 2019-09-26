using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity3_0.Models
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The name cannot exceed 30 characters nor be less than 2.")]
        public string Name { get; set;}

        [Required]
        [Range(1, 100_000_000_000, ErrorMessage = "The population cannot exceed 100 Billion nor be less than 1.")]
        public string Population { get; set; }

        public List<City> Cities { get; set; }
    }
}
