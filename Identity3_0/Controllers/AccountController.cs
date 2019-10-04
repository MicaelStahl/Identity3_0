using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInUser user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var appUser = await _userManager.FindByNameAsync(user.Email);

            var result = await _signInManager.PasswordSignInAsync(appUser, user.Password, false, false);

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
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index), "Home");
        }

        #endregion SignIn / SignOut

        #region Create

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(bool created = false)
        {
            if (created)
                ViewBag.created = "User was successfully created.";

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
                    ModelState.AddModelError(user.Email, "The Email is already in use.");

                    ViewBag.error = "The Email is already in use.";

                    return View();
                }

                user.UserName = user.Email; // Should in theory not be needed considering the configuration for AppUser.

                var result = await _userManager.CreateAsync(user, user.Password);

                if (result.Succeeded)
                {
                    // Send an email verification here later.

                    // Checks if the IsAdmin value is true and adds the user to the administrator role if it is.
                    var roleResult = user.IsAdmin ?
                        await _userManager.AddToRolesAsync(user, new List<string> { "Administrator", "NormalUser" }) :
                        await _userManager.AddToRoleAsync(user, "NormalUser");

                    // If user is administrator, then return to same view incase admin wants to create more users.
                    if (User.IsInRole("Administrator"))
                    {
                        return RedirectToAction(nameof(Register), new { created = true }); // Allows the admin to create more users.
                    }

                    return RedirectToAction(nameof(SignIn)); // Redirect to SignIn so user can sign in.
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
        public async Task<IActionResult> Profile(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Could not find the requested user.");

                return NotFound(ModelState);
            }

            if (User.IsInRole("Administrator"))
            {
                var roles = await _userManager.GetRolesAsync(user);

                return View(new FrontUser { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Age = user.Age, Email = user.Email, IsAdmin = user.IsAdmin, PhoneNumber = user.PhoneNumber, Roles = roles.ToList() });
            }

            return View(new FrontUser { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Age = user.Age, Email = user.Email, IsAdmin = user.IsAdmin, PhoneNumber = user.PhoneNumber });
        }

        #endregion Find

        #region Edit-Section

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FrontUser user)
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

            original.FirstName = user.FirstName;
            original.LastName = user.LastName;
            original.Age = user.Age;
            original.Email = user.Email;
            original.PhoneNumber = user.Email;

            // Using Discard " _ " to make this expression a possibility.
            _ = user.IsAdmin == original.IsAdmin ? // If IsAdmin was not changed
                null : // Do nothing
                user.IsAdmin == false && original.IsAdmin == true ? // Else if IsAdmin was changed to false
                await _userManager.RemoveFromRoleAsync(original, "Administrator") : // Remove from administrator role
                user.IsAdmin == true && original.IsAdmin == false ? // Else if IsAdmin was changed to true
                await _userManager.AddToRoleAsync(original, "Administrator") : // Add to administrator role
                null; // Else do nothing.

            original.IsAdmin = user.IsAdmin;

            var result = await _userManager.UpdateAsync(original);

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

        #region EditUserEmail

        [HttpGet]
        public IActionResult EditUserEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserEmail(ChangeUserEmail userEmail)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please fill all fields.");

                return View();
            }

            if (await _userManager.FindByEmailAsync(userEmail.OldEmail) == null)
            {
                ModelState.AddModelError(userEmail.OldEmail, "The old email is not in use.");

                return View();
            }

            if (userEmail.OldEmail == userEmail.NewEmail)
            {
                ModelState.AddModelError(string.Empty, "The new email can not be the same as the old one.");

                return View();
            }

            var result = await _userManager.GenerateChangeEmailTokenAsync(await _userManager.FindByEmailAsync(userEmail.OldEmail), userEmail.NewEmail);

            if (string.IsNullOrWhiteSpace(result))
            {
            }
            // Send email verification to the new email.

            return View();
        }

        #endregion EditUserEmail

        #region EditUserPassword

        [HttpGet]
        public async Task<IActionResult> EditUserPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Could not find the requested user.");

                return NotFound(ModelState);
            }

            ViewBag.Id = user.Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserPassword(ChangeUserPassword userPassword)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please fill all fields.");

                return View(userPassword);
            }

            if (userPassword.OldPassword == userPassword.NewPassword)
            {
                ModelState.AddModelError(string.Empty, "Cannot use the same password as a previous one.");

                return View(userPassword);
            }

            var result = await _userManager.ChangePasswordAsync(await _userManager.FindByIdAsync(userPassword.Id), userPassword.OldPassword, userPassword.NewPassword);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Profile), "Account", new { email = User.Identity.Name });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(userPassword);
            }
        }

        #endregion EditUserPassword

        #endregion Edit-Section

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

            var roles = await _userManager.GetRolesAsync(user);

            return View(new FrontUser { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Age = user.Age, Email = user.Email, IsAdmin = user.IsAdmin, PhoneNumber = user.PhoneNumber, Roles = roles.ToList() });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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