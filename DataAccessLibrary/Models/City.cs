using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class City
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The name cannot exceed 50 characters nor be less than 2.")]
        public string Name { get; set; }

        [Required]
        [Range(1, 1_000_000_000, ErrorMessage = "The population cannot exceed 1,000,000,000 (1 Billion) Nor be less than 1.")]
        public string Population { get; set; }

        [Required]
        [RegularExpression(@"^\d{1,8}\-{0,1}\d{0,8}$", ErrorMessage = "Valid Patterns: xxxxx & xxxxx-xxxxx \nMin length: x or x-x \nMax length: xxxxxxxx or xxxxxxxx-xxxxxxxx ")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "The postalcode cannot exceed 8 character-length nor be less than 4 character-length.")]
        public string PostalCode { get; set; }

        public List<Person> People { get; set; }

        public Country Country { get; set; }
    }
}