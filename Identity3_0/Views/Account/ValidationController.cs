using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Identity.Views.Account
{
    public class ValidationController : Controller
    {
        #region D.I

        private readonly IAccountValidation _validate;

        public ValidationController(IAccountValidation validate)
        {
            _validate = validate;
        }

        #endregion D.I

        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyEmail(string email)
        {
            if (!_validate.VerifyEmail(email))
            {
                return Json($"Email {email} is already in use.");
            }

            return Json(true);
        }
    }
}