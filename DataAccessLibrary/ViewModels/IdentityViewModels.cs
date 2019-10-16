using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibrary.ViewModels
{
    /// <summary>
    /// The default User for the application.
    /// </summary>
    [ProtectedPersonalData]
    public class AppUser : IdentityUser
    {
        // This automatically sets the username to be the same as the given Email.
        //[EmailAddress]
        //[DataType(DataType.EmailAddress)]
        //public override string UserName { get => base.UserName; set => base.UserName = Email; }

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
        [Remote(action: "VerifyEmail", controller: "Validation")] // Enables quick authentication via ajax
        public override string Email { get; set; } // Overriding to get the email the way I want it.

        [Required]
        [Phone]
        [Display(Name = "Phonenumber")]
        [DataType(DataType.PhoneNumber)]
        public override string PhoneNumber { get; set; } // Overriding to get the phonenumber the way I want it.

        /// <summary>
        /// Indicates whether the user is an admin or not.
        /// </summary>
        [Display(Name = "Admin")]
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
        [Display(Name = "Password confirmation")]
        public string PasswordConfirmation { get; set; }
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
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The password cannot exceed 20 characters nor be less than 8.")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// A viewmodel used for front-end purposes.
    /// </summary>
    [ProtectedPersonalData]
    public class FrontUser
    {
        [Key]
        public string Id { get; set; }

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
        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "User roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>
    /// Viewmodel containing parameters for changing email.
    /// </summary>
    public class ChangeUserEmail
    {
        [Display(Name = "Current email")]
        public string OldEmail { get; set; }

        [Required]
        [Compare("NewEmail", ErrorMessage = "The emails do not match.")]
        public string CompareEmail { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "New Email")]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; }
    }

    /// <summary>
    /// Viewmodel containing parameters for changing passwords.
    /// </summary>
    public class ChangeUserPassword
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The password cannot exceed 20 characters nor be less than 8.")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The password cannot exceed 20 characters nor be less than 8.")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [Display(Name = "Password confirmation")]
        public string PasswordConfirmation { get; set; }
    }

    public class ForgottenUserPassword
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class PasswordResetting
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}