using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity3_0.Models
{
    public class Person
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The firstname cannot exceed 30 character nor be less than 2.")]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The lastname cannot exceed 30 character nor be less than 2.")]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Required]
        [Range(0, 110, ErrorMessage = "The age cannot be greater than 110.")]
        public int Age { get; set; }

        [Required]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phonenumber")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public City City { get; set; }
    }
}