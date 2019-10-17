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

            var result = await _service.FindAll();

            if (result.Message == ActionMessages.Success)
            {
                return View(result.Countries);
            }
            else if (result.Message == ActionMessages.Empty)
            {
                ViewBag.error = "No countries were found. Please update your page if this is incorrect.";

                return View();
            }
            else
            {
                return BadRequest();
            }
        }

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]Country country)
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
                ModelState.AddModelError(string.Empty, "Unexpected error: The action failed.");

                return BadRequest(ModelState);
            }
        }

        #endregion Create

        #region Find

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, string message = null)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError("id", "Unexpected error occurred: ID was blank.");

                return RedirectToAction(nameof(Index), new { message = string.Empty, error = "Unexpected error occurred: ID was blank." });
            }

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            {
                if (!string.IsNullOrWhiteSpace(message))
                    ViewBag.message = message;

                return View(result.Country);
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: No country was found with the ID: {id}");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unexpected error: The action failed.");

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

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            {
                return View(result.Country);
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: No country was found with the ID: {id}");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unexpected error: The action failed.");

                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm]Country country)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: Not all fields were valid.");

                return View(country);
            }

            var result = await _service.Edit(country);

            if (result.Message == ActionMessages.Updated)
            {
                return RedirectToAction(nameof(Details), new { message = $"{result.Country.Name} was successfully updated." });
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: No country with ID {country.Id} was found.");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unexpected error: The action failed.");

                return BadRequest(ModelState);
            }
        }

        #endregion Edit

        #region AddCities

        [HttpGet]
        public async Task<IActionResult> AddCitiesToCountry(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: The ID was blank.");

                return BadRequest(new NullReferenceException("Unexpected error occurred: The ID was blank.").Message);
            }

            var country = await _service.Find(id);

            if (country.Message == ActionMessages.Success)
            {
                return View(new AddCitiesToCountry { Cities = await _list.GetCityList(country.Country), Country = new KeyValuePair<Guid, string>(country.Country.Id, country.Country.Name) });
            }
            else if (country.Message == ActionMessages.NotFound)
            {
                return RedirectToAction(nameof(Index), new { message = string.Empty, error = $"No country was found with ID: {id}" });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unexpected error: The action failed.");

                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCitiesToCountry([FromForm]AddCitiesToCountryVM country)
        {
            if (country.CountryId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: The ID was blank.");

                return BadRequest(ModelState);
            }

            var result = await _service.AddCitiesToCountry(country);

            if (result.Message == ActionMessages.Updated)
            {
                var cities = country.CitiesId.Count > 1 ? "cities" : "city";

                return RedirectToAction(nameof(Details), new { id = result.Country.Id, message = $"The requested {cities} were successfully added to {result.Country.Name}." });
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: Could not find any country with the ID: {country.CountryId}");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Something unexpected happened. The requested action: {result.Message.ToString()}");

                return BadRequest(ModelState);
            }
        }

        #endregion AddCities

        #region RemoveCities

        [HttpGet]
        public async Task<IActionResult> RemoveCitiesFromCountry(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: The ID was blank.");

                return BadRequest(ModelState);
            }

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            {
                return View(result.Country);
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Not country was found with ID: {id}.");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: The action {result.Message.ToString()}.");

                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCitiesFromCountry([FromForm]RemoveCitiesFromCountryVM country)
        {
            if (country.CountryId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: The ID was blank.");

                return BadRequest(ModelState);
            }

            var result = await _service.RemoveCitiesFromCountry(country);

            if (result.Message == ActionMessages.Updated)
            {
                var cities = country.CitiesId.Count > 1 ? "cities" : "city";

                return RedirectToAction(nameof(Details), new { id = result.Country.Id, message = $"The requested {cities} were successfully removed from {result.Country.Name}." });
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Not country was found with ID: {country.CountryId}.");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error occurred: The action {result.Message.ToString()}.");

                return BadRequest(ModelState);
            }
        }

        #endregion RemoveCities

        #endregion Edit - Section

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: The ID was blank.");

                return BadRequest(ModelState);
            }

            var country = await _service.Find(id);

            if (country.Message == ActionMessages.Success)
            {
                return View(country.Country);
            }
            else if (country.Message == ActionMessages.NotFound)
            {
                return RedirectToAction(nameof(Index), new { message = string.Empty, error = $"No country with ID: {id} was found." });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unexpected error: The action failed.");

                return BadRequest(ModelState);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred: The ID was blank.");

                return BadRequest(ModelState);
            }

            var result = await _service.Delete(id);

            if (result == ActionMessages.Deleted)
            {
                return RedirectToAction(nameof(Index), new { message = "The requested country was successfully removed." });
            }
            else if (result == ActionMessages.NotFound)
            {
                return RedirectToAction(nameof(Index), new { message = string.Empty, error = "The requested country could not be found. Please update your page and try again." });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unexpected error: The action failed.");

                return BadRequest(ModelState);
            }
        }

        #endregion Delete
    }
}