using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity3_0.ViewModels
{
    /// <summary>
    /// The default User for the application.
    /// </summary>
    [ProtectedPersonalData]
    public class AppUser : IdentityUser
    {
        [Required]
        [Display(Name = "Firstname")]
        [StringLength(30, MinimumLength =2, ErrorMessage = "The firstname cannot exceed 30 characters nor be less than 2.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Lastname")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The lastname cannot exceed 30 characters nor be less than 2.")]
        public string LastName { get; set; }

        [Required]
        [Range(15, 110, ErrorMessage = "The age cannot exceed 110 years old nor be less than 15.")]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; } // Overriding to get the email the way I want it.

        [Required]
        [Phone]
        [Display(Name = "Phonenumber")]
        [DataType(DataType.PhoneNumber)]
        public override string PhoneNumber { get; set; } // Overriding to get the phonenumber the way I want it.

        /// <summary>
        /// Indicates whether the user is an admin or not.
        /// </summary>
        public bool IsAdmin { get; set; }
    }

    /// <summary>
    /// A viewmodel used for registering new users.
    /// </summary>
    public class RegisterUser : AppUser
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The password cannot exceed 20 characters nor be less than 8.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ComparePassword { get; set; }
    }

    /// <summary>
    /// A viewmodel used for signing in users.
    /// </summary>
    public class SignInUser
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } // This application uses emails to sign in.

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The password cannot exceed 20 characters nro be less than 8.")]
        public string Password { get; set; }
    }

    /// <summary>
    /// A viewmodel used for front-end purposes.
    /// </summary>
    [ProtectedPersonalData]
    public class FrontUser
    {
        [Required]
        [Display(Name = "Firstname")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The firstname cannot exceed 30 characters nor be less than 2.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Lastname")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "The lastname cannot exceed 30 characters nor be less than 2.")]
        public string LastName { get; set; }

        [Required]
        [Range(15, 110, ErrorMessage = "The age cannot exceed 110 years old nor be less than 15.")]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phonenumber")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Indicates whether the user is an admin or not.
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
