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

        public bool VerifyEmail(string email)
        {
            if (_userManager.FindByEmailAsync(email).Result == null) // Indicates the requested email does not exist in database.
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}