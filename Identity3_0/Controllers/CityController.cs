using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLibrary.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Identity.Controllers
{
    public class CityController : Controller
    {
        #region D.I

        private readonly ICityRepository _service;
        private readonly IGlobalRepository _list;

        public CityController(ICityRepository service, IGlobalRepository list)
        {
            _service = service;
            _list = list;
        }

        #endregion D.I

        public async Task<IActionResult> Index(string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
                ViewBag.message = $"The requested city was successfully {message.ToLower()}!";

            return View(await _service.FindAll());
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new CityCreation { Countries = await _list.GetCountries() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody]City city, Guid? countryId)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "All fields were not filled.");

                return BadRequest(ModelState);
            }

            var result = await _service.Create(city, countryId);

            if (result == ActionMessages.Created)
            {
                return RedirectToAction(nameof(Index), "City", new { message = result.ToString() });
            }
            else if (result == ActionMessages.FillAllFields)
            {
                return BadRequest(ModelState);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        #endregion Create

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, bool updated = false)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest();
            }

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            {
                if (updated)
                {
                    ViewBag.message = $"{result.City.Name} was successfully updated.";
                }
                return View(result.City);
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "City could not be found.");

                return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest();
            }

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            {
                return View(new CityCreation { City = result.City, Countries = await _list.GetCountries() });
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "City could not be found.");

                return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody]City city, Guid? countryId)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "The data was invalid. please try again.");

                return View();
            }

            var result = await _service.Edit(city, countryId);

            if (result.Message == ActionMessages.Updated)
            {
                return RedirectToAction(nameof(Details), "City", new { id = result.City.Id, updated = true });
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "The city could not be found.");

                return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddPeople(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");

                return BadRequest();
            }

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            {
                return View(); // Create new viewmodel here for 1 City with a list of homeless people
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "City could not be found.");

                return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}