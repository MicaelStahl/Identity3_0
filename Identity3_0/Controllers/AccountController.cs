using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity3_0.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Identity.Controllers
{
    /// <summary>
    /// Decide if I want to create a repository for this later.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {

        #region D.I

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #endregion
        
        public IActionResult Index()
        {
            return View();
        }

        #region SignIn / SignOut

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

            if (result.Succeeded)
            {
                return LocalRedirect(nameof(Index));
            }
            else if (result.IsLockedOut)
            {
                ViewBag.message = "User is locked out. Please try again later.";
                return View();
            }
            else
            {
                ViewBag.message = "Something went wrong. Please check your inputs then try again.";
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return LocalRedirect(nameof(Index));
        }

        #endregion

        #region Create

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create() // Register
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(RegisterUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Please fill all fields and try again.");
                    throw new Exception();
                }

                if (await _userManager.FindByEmailAsync(user.Email) != null)
                {
                    return BadRequest("The Email is already in use.");
                }

                user.UserName = user.Email;

                var result = await _userManager.CreateAsync(user, user.Password);

                if (result.Succeeded)
                {
                    // Send an email verification here later.

                    return LocalRedirect(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    throw new Exception();
                }

            }
            catch (Exception)
            {
                return BadRequest(ModelState);
            }
        }

        #endregion

    }
}