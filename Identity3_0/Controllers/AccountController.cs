using BusinessLibrary.Interfaces;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
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
        private readonly IEmailSenderUpdated _emailSender;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSenderUpdated emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        #endregion D.I

        public IActionResult Index()
        {
            return View();
        }

        #region EmailValidation

        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (await _userManager.FindByEmailAsync(email) == null)
                throw new ArgumentNullException("Unexpected error occurred: The created user could not be found.");

            ViewBag.email = email;

            return View();
        }

        /// <summary>
        /// Only accessible for the active user. Used after registration if the registered user isn't an admin.
        /// NOTE: MIGHT NOT BE USED AT ALL
        /// </summary>
        /// <param name="email">The email to send the mail to.</param>
        /// <returns></returns>
        //[HttpGet]
        //public async Task<IActionResult> SendEmailConfirmation(string email)
        //{
        //    if (string.IsNullOrWhiteSpace(email))
        //    {
        //        ModelState.AddModelError(string.Empty, $"Unexpected error occurred: The email is blank.");

        //        return BadRequest(ModelState);
        //    }

        //    var user = await _userManager.GetUserAsync(User);

        //    if (user == null)
        //    {
        //        return NotFound("And error occurred: Could not find the active user.");
        //    }
        //    var userEmail = await _userManager.GetEmailAsync(user);

        //    if (userEmail != email)
        //    {
        //        var owner = await _userManager.FindByEmailAsync(email);

        //        if (owner != null && !string.Equals(await _userManager.GetUserIdAsync(owner), await _userManager.GetUserIdAsync(user)))
        //        {
        //            ModelState.AddModelError(string.Empty, new IdentityErrorDescriber().DuplicateEmail(email).Description);

        //            return NotFound($"Cannot find the active user with the given Email of {email}.");
        //        }
        //    }

        //    // Creates token for user
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //    // Creates the url back to the application for the user with the given values
        //    var callbackUrl = Url.Action(new UrlActionContext
        //    {
        //        Action = nameof(ConfirmEmail),
        //        Controller = "Account",
        //        Values = new { userId = user.Id, token },
        //        Protocol = Request.Scheme = "https",
        //        Host = "localhost:44351"
        //    });

        //    // Creates a default message for the user.
        //    var message = await _emailSender.EmailVerificationMessageAsync(callbackUrl, Request, Url);

        //    // Sends the email with the given parameters.
        //    await _emailSender.SendEmailAsync(email, "Confirm your email", message);

        //    return RedirectToAction(nameof(CheckEmail), new { email });
        //}

        /// <summary>
        /// Only accessible for the active user. Aka the admin can't do this for other users.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ResendEmailVerification(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: Email was blank.");

                return BadRequest(ModelState); // Change this sometime.
            }

            // Continue here https://www.codeproject.com/Articles/1272172/Require-Confirmed-Email-in-ASP-NET-Core-2-2-Part-1

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "The user could not be found.");

                return BadRequest(ModelState);
            }

            //var userEmail = await _userManager.GetEmailAsync(user);

            //if (userEmail != email)
            //{
            //    var owner = await _userManager.FindByEmailAsync(email);

            // Checks whether the email sent to the method is for the same user as the active user.
            //    if (owner != null && !string.Equals(await _userManager.GetUserIdAsync(owner),
            //                                        await _userManager.GetUserIdAsync(user)))
            //    {
            //        ModelState.AddModelError(string.Empty,
            //            new IdentityErrorDescriber()
            //            .DuplicateEmail(email)
            //            .Description);

            //        // Return to Home screen
            //        return RedirectToAction(nameof(Index), "Home");
            //    }

            //    // Unsure if this is correct.
            //    await _userManager.SetEmailAsync(user, email);
            //}

            var result = await _userManager.UpdateSecurityStampAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction(nameof(Index));
            }

            // Generates a confirmationtoken for user.
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Generates the url inside the mail itself.
            var callbackUrl = Url.Action(new UrlActionContext
            {
                Action = nameof(ConfirmEmail),
                Controller = "Account",
                Values = new { userId = user.Id, token },
                Protocol = Request.Scheme = "https",
                Host = "localhost:44351"
            });

            // The default message for the mail.
            var message = await _emailSender.EmailVerificationMessageAsync(callbackUrl, Request, Url);

            // Sends the mail to the user.
            await _emailSender.SendEmailAsync(email, "Confirm your email", message);

            return RedirectToAction(nameof(CheckEmail), new { email });
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError(string.Empty,
                    new IdentityErrorDescriber()
                    .InvalidToken()
                    .Description);

                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID {user.Id}.");
            }

            var email = await _userManager.GetEmailAsync(user);

            var owner = await _userManager.FindByEmailAsync(email);

            if (owner != null && !string.Equals(await _userManager.GetUserIdAsync(owner), userId))
            {
                ModelState.AddModelError(string.Empty,
                    new IdentityErrorDescriber()
                    .DuplicateEmail(email)
                    .Description);

                // Return to RescendEmail
                return RedirectToAction(nameof(ResendEmailVerification), "Account", new { userId = user.Id, error = $"Unable to load user with ID {_userManager.GetUserId(User)}." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }
            else
            {
                // Redirect to login if the user is not signed in.
                if (User.Identity.IsAuthenticated && !User.IsInRole("Administrator"))
                {
                    return RedirectToAction(nameof(Profile), new { email = user.Email, message = "The email was successfully verfied." });
                }

                // If Administrator was the one sending the mail, then the user will temporarily be signed in as admin.
                // This is to stop that.
                if (User.IsInRole("Administrator"))
                    await _signInManager.SignOutAsync();

                return RedirectToAction(nameof(SignIn));
            }
        }

        #endregion EmailValidation

        #region ForgottenPassword

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgottenPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgottenPassword(ForgottenUserPassword userPassword)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: the given email of: {userPassword.Email} is invalid.");

                ViewBag.error = "Unexpected error occurred: The given email was invalid.";

                return View();
            }

            var user = await _userManager.FindByEmailAsync(userPassword.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: No user exists with the given email of: {userPassword.Email}.");

                ViewBag.error = $"Could not find a user with the given email of {userPassword.Email}." +
                    $"<br />" +
                    $"If you do not have an account, then please click <a asp-action='Register' asp-controller='Account'>here</a>";

                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Url for the callback for the mail.
            var callbackUrl = Url.Action(new UrlActionContext
            {
                Action = nameof(PasswordReset),
                Controller = "Account",
                Values = new { userId = user.Id, email = user.Email, token },
                Protocol = Request.Scheme = "https",
                Host = "localhost:44351"
            });

            // The message for the mail.
            var htmlMessage = "<h2>Reset password</h2>" +
                "<hr />" +
                $"Click the link to reset your current password. <a href='{callbackUrl}'>Reset Password</a>" +
                "<br /><br /><br />" +
                "<em>If you didn't request this email, then please ignore this mail.</em>";

            // Send mail.
            await _emailSender.SendEmailAsync(user.Email, "Reset password", htmlMessage);

            // Redirects to PasswordResetWait to indicate the mail was successfully sent.
            return RedirectToAction(nameof(PasswordResetWait), new { email = user.Email });
        }

        /// <summary>
        /// Method that will only ever be used after a user has requested a password reset.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("[controller]/Reset-Password")]
        public IActionResult PasswordResetWait(string email)
        {
            ViewBag.email = email; // Only used if the user re-sends the password reset mail.
            return View();
        }

        // NOTE: Create a RescendPasswordReset here later.

        [HttpGet]
        [AllowAnonymous]
        [Route("[controller]/Password-Reset")]
        public IActionResult PasswordReset(string userId, string email, string token)
        {
            return View(new PasswordResetting { UserId = userId, Email = email, Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordReset(PasswordResetting reset)
        {
            if (string.IsNullOrWhiteSpace(reset.UserId) || string.IsNullOrWhiteSpace(reset.Email))
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(reset.Token))
            {
                ModelState.AddModelError(string.Empty, new IdentityErrorDescriber().InvalidToken().Description);

                return RedirectToAction(nameof(Index));
            }

            if (reset.Password != reset.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, new IdentityErrorDescriber().PasswordMismatch().Description);

                return View(reset);
            }

            var user = await _userManager.FindByIdAsync(reset.UserId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: Could not find a user with the ID: {reset.UserId}.");

                return BadRequest(ModelState);
            }

            if (user.Email != reset.Email)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: Could not verify the user.");

                return BadRequest(ModelState);
            }
            var result = await _userManager.ResetPasswordAsync(user, reset.Token, reset.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(SignIn), new { message = "Your password was successfully reset" });
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

        #endregion ForgottenPassword

        #region SignIn / SignOut

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn(string message = null, string error = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
                ViewBag.message = message;

            if (!string.IsNullOrWhiteSpace(error))
                ViewBag.error = error;

            //return PartialView("_SignInpartial");
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

            if (appUser == null)
            {
                ViewBag.error = "The username or password was invalid.";
                return PartialView("_SignInPartial");
            }

            var result = await _signInManager.PasswordSignInAsync(appUser, user.Password, user.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            else if (result.IsLockedOut)
            {
                ViewBag.error = "User is locked out. Please try again later.";
                return View();
            }
            else
            {
                ViewBag.error = "Invalid username or password.";
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

                    return View(user);
                }

                if (await _userManager.FindByEmailAsync(user.Email) != null)
                {
                    ModelState.AddModelError(user.Email, "The Email is already in use.");

                    ViewBag.error = "The Email is already in use.";

                    return View(user);
                }

                user.UserName = user.Email; // Should in theory not be needed considering the configuration for AppUser.

                var result = await _userManager.CreateAsync(user, user.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var createdUser = await _userManager.FindByEmailAsync(user.Email);

                    if (createdUser == null)
                    {
                        throw new InvalidOperationException("Unexpected error occured: Could not find the created user.");
                    }

                    // Creates the url back to the application.
                    var callbackUrl = Url.Action(new UrlActionContext
                    {
                        Action = nameof(ConfirmEmail),
                        Controller = "Account",
                        Values = new { userId = createdUser.Id, token },
                        Protocol = Request.Scheme = "https",
                        Host = "localhost:44351"
                    });

                    // Creates a default message for the user.
                    var htmlMessage = await _emailSender.EmailVerificationMessageAsync(callbackUrl, Request, Url);

                    // Sends the email with the given parameters.
                    await _emailSender.SendEmailAsync(createdUser.Email, "Confirm your email", htmlMessage);

                    // Checks if the IsAdmin value is true and adds the user to the administrator role if it is.
                    var roleResult = user.IsAdmin ?
                        await _userManager.AddToRolesAsync(user, new string[] { "Administrator", "NormalUser" }) :
                        await _userManager.AddToRoleAsync(user, "NormalUser");

                    // If user is administrator, then return to same view incase admin wants to create more users.
                    if (User.IsInRole("Administrator"))
                    {
                        return RedirectToAction(nameof(Register), new { created = true }); // Allows the admin to create more users.
                    }

                    return RedirectToAction(nameof(CheckEmail)); // Redirect to CheckEmail so user will feel implied to verify the email.
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion Create

        #region Find

        [HttpGet]
        public async Task<IActionResult> Profile(string email, string message = null)
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

            if (!string.IsNullOrWhiteSpace(message))
                ViewBag.message = message;

            return View(new FrontUser { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Age = user.Age, Email = user.Email, IsAdmin = user.IsAdmin, PhoneNumber = user.PhoneNumber });
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Users()
        {
            return View(await _userManager.Users.ToListAsync());
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
        public IActionResult EditUserEmail(string email)
        {
            return View(new ChangeUserEmail { OldEmail = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserEmail(ChangeUserEmail userEmail)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: Not all fields fulfilled the required format of {ModelState}.");

                return View();
            }

            if (await _userManager.FindByEmailAsync(userEmail.OldEmail) == null)
            {
                ModelState.AddModelError(string.Empty, "Unexpected Error Occurred: No user exists with this email.");

                ViewBag.error = $"Unexpected error occurred: Could not find user with the given email: {userEmail.OldEmail}.";

                return View();
            }

            if (userEmail.OldEmail != User.Identity.Name)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: Something went wrong.");

                return View();
            }

            if (userEmail.OldEmail == userEmail.NewEmail)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: The new email can not be the same as the old one.");

                ViewBag.error = "Unexpected error occurred: The new email cannot be the same as the old one.";

                return View();
            }

            if (await _userManager.FindByEmailAsync(userEmail.NewEmail) != null)
            {
                ModelState.AddModelError(string.Empty, new IdentityErrorDescriber().DuplicateEmail(userEmail.NewEmail).Description);

                ViewBag.error = "Unexpected error occurred: Invalid email.";

                return View();
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(await _userManager.FindByEmailAsync(userEmail.OldEmail), userEmail.NewEmail);

            if (string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError(string.Empty, new IdentityErrorDescriber().InvalidToken().Description);

                ViewBag.error = "Unexpected error occurred: Could not token. Did you submit a valid email?";

                return View();
            }

            var callbackUrl = Url.Action(new UrlActionContext
            {
                Action = nameof(VerifyEmailChangeTokenAsync),
                Controller = "Account",
                Values = new
                {
                    userId = await _userManager.GetUserIdAsync(await _userManager.FindByEmailAsync(userEmail.NewEmail)),
                    email = userEmail.NewEmail,
                    token
                },
                Protocol = Request.Scheme = "https",
                Host = "localhost:44351"
            });

            var htmlMessage = await _emailSender.EmailVerificationMessageAsync(callbackUrl, Request, Url);

            await _emailSender.SendEmailAsync(userEmail.NewEmail, "Confirm your email", htmlMessage);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmailChangeTokenAsync(string userId, string email, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: Something went wrong.");

                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                ModelState.AddModelError(string.Empty, new IdentityErrorDescriber().InvalidToken().Description);

                return BadRequest(ModelState); // change this later.
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: Cannot find user with the given ID: {userId}");

                return NotFound(ModelState);
            }

            var result = await _userManager.ChangeEmailAsync(user, email, token);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Profile), new { email, message = "The email was successfully confirmed" });
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

        #endregion EditUserEmail

        #region EditUserPassword

        [HttpGet]
        public IActionResult EditUserPassword(string id)
        {
            ViewBag.UserId = id;

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
                ModelState.AddModelError(userPassword.OldPassword, "Unexpected error occurred: The new and old password cannot match.");

                return View(userPassword);
            }

            var user = await _userManager.FindByIdAsync(userPassword.UserId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: Could not find user with ID: {userPassword.UserId}.");

                return NotFound(ModelState);
            }

            var result = await _userManager.ChangePasswordAsync(user, userPassword.OldPassword, userPassword.NewPassword);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Profile), "Account", new { email = user.Email });
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