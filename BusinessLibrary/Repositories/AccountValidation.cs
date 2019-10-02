using BusinessLibrary.Interfaces;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLibrary.Repositories
{
    public class AccountValidation : IAccountValidation
    {
        #region D.I

        private readonly UserManager<AppUser> _userManager;

        public AccountValidation(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        #endregion D.I

        /// <summary>
        /// Used for verifying User emails before submitting.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool VerifyEmail(string email)
        {
            if (_userManager.FindByEmailAsync(email).Result == null) // checks if the requested email does not exist in database.
            {
                return true;    // Indicates the requested email does not exist in database.
            }
            else
            {
                return false;   // Indicates the requested email does exist in database.
            }
        }
    }
}