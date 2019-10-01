using Identity3_0.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        #endregion D.I

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
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index), "Home");
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

            return RedirectToAction(nameof(Index), "Home");
        }

        #endregion SignIn / SignOut

        #region Create

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Please fill all fields and try again.");

                    return View();
                }

                if (await _userManager.FindByEmailAsync(user.Email) != null)
                {
                    ModelState.AddModelError(string.Empty, "The Email is already in use.");

                    ViewBag.error = "The Email is already in use.";

                    return View();
                }

                //user.UserName = user.Email; // Should in theory not be needed considering the configuration for AppUser.

                var result = await _userManager.CreateAsync(user, user.Password);

                if (result.Succeeded)
                {
                    // Send an email verification here later.

                    // Checks if the IsAdmin value is true and adds the user to the administrator role if it is.
                    var roleResult = user.IsAdmin ?
                        await _userManager.AddToRolesAsync(user, new List<string> { "Administrator", "NormalUser" }) :
                        await _userManager.AddToRoleAsync(user, "NormalUser");

                    return LocalRedirect(nameof(SignIn)); // Redirect to SignIn so user can sign in.
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

        #endregion Create

        #region Find

        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Could not find the requested user.");

                return NotFound(ModelState);
            }

            return View(new FrontUser { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Age = user.Age, Email = user.Email, IsAdmin = user.IsAdmin, PhoneNumber = user.PhoneNumber });
        }

        #endregion Find

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Could not find the requested user.");

                return NotFound(ModelState);
            }

            return View(new FrontUser { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Age = user.Age, Email = user.Email, IsAdmin = user.IsAdmin, PhoneNumber = user.PhoneNumber });
        }

        /// <summary>
        /// Expand this Edit to be more dynamic for specifically Email, Password etc.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(AppUser user)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please fill all fields.");

                return BadRequest(ModelState);
            }

            var original = await _userManager.FindByIdAsync(user.Id);

            if (original == null)
            {
                ModelState.AddModelError(string.Empty, "Could not find the original.");

                return NotFound(ModelState);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Profile), "Account", new { id = user.Id });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }
        }

        #endregion Edit

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Could not find the requested user.");

                return NotFound(ModelState);
            }

            return View(new FrontUser { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Age = user.Age, Email = user.Email, IsAdmin = user.IsAdmin, PhoneNumber = user.PhoneNumber });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest(ModelState);
            }

            var result = await _userManager.DeleteAsync(await _userManager.FindByIdAsync(id));

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }
        }

        #endregion Delete
    }
}