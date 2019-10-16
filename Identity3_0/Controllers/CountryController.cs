using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLibrary.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Identity.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("World/[controller]/[action]")]
    public class CountryController : Controller
    {
        #region D.I

        private readonly ICountryRepository _service;
        private readonly IGlobalRepository _list;

        public CountryController(ICountryRepository service, IGlobalRepository list)
        {
            _service = service;
            _list = list;
        }

        #endregion D.I

        public async Task<IActionResult> Index(string message = null, string error = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
                ViewBag.message = message;

            if (!string.IsNullOrWhiteSpace(error))
                ViewBag.error = error;

            return View(await _service.FindAll());
        }

        #region GetOneCountry

        /// <summary>
        /// A method used for all methods apart from "Find" that wants to get one user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        protected internal async Task<Country> GetOneCountry(Guid id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Something went wrong.");
                }

                var result = await _service.Find(id);

                if (result.Message == ActionMessages.Success)
                {
                    return result.Country;
                }
                else if (result.Message == ActionMessages.NotFound)
                {
                    throw new Exception($"No country was found with the given ID: {id}");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
        }

        #endregion GetOneCountry

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "One or more fields were invalid.");

                return View(country);
            }

            var result = await _service.Create(country);

            if (result == ActionMessages.Created)
            {
                return RedirectToAction(nameof(Index), new { message = $"{country.Name} was successfully created!" });
            }
            else if (result == ActionMessages.FillAllFields)
            {
                ModelState.AddModelError(string.Empty, "Not all fields were filled correctly.");

                return View(country);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        #endregion Create

        #region Find

        [HttpGet]
        public async Task<IActionResult> Find(Guid id, string redirectUrl = null)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError("id", "Unexpected error occurred: ID was blank.");

                return RedirectToAction(nameof(Index), new { message = string.Empty, error = "Unexpected error occurred: ID was blank." });
            }

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            { // If this redirectUrl string isn't null, it then redirects to that given string.
                if (!string.IsNullOrWhiteSpace(redirectUrl))
                    return RedirectToAction(redirectUrl, new { country = result.Country });

                return View(result.Country);
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: No country was found with the ID: {id}");

                return NotFound(ModelState);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        #endregion Find

        #region Edit - Section

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: The ID was blank.");

                return RedirectToAction(nameof(Index), new { message = string.Empty, error = "Unexpected error occurred: The ID was blank." });
            }

            var result = await GetOneCountry(id);

            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong when fetching country");

                return BadRequest(ModelState);
            }

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: Not all fields were valid.");

                return View(country);
            }

            var result = await _service.Edit(country);

            if (result.Message == ActionMessages.Updated)
            {
                return RedirectToAction(nameof(Find), new { message = $"{result.Country.Name} was successfully updated." });
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: No country with ID {country.Id} was found.");

                return NotFound(ModelState);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        #endregion Edit

        #endregion Edit - Section
    }
}