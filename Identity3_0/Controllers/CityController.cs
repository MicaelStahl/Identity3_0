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
    [Route("World/[controller]/[action]")]
    [Authorize(Roles = "Administrator")]
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
        public async Task<IActionResult> Create([FromForm]City city, Guid? countryId)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(city.Name) || string.IsNullOrWhiteSpace(city.Population) || string.IsNullOrWhiteSpace(city.PostalCode))
                {
                    ModelState.AddModelError(string.Empty, "All fields were not filled.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid input. Please check the fields and try again.");
                }

                return View(new CityCreation { Countries = await _list.GetCountries(), City = city });
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

        #region Details

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

        #endregion Details

        #region Edit

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
        public async Task<IActionResult> Edit([FromForm]City city, Guid? countryId)
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

        #region AddPeople

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
                return View(new AddPeopleToCity { City = new KeyValuePair<Guid, string>(result.City.Id, result.City.Name), People = await _list.PeopleNotInCity(result.City) });
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
        public async Task<IActionResult> AddPeople([FromForm]AddPeopleToCityVM city)
        {
            if (city.CityId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "The Id could not be fetched.");

                return BadRequest(ModelState);
            }

            var result = await _service.AddPeople(city.CityId, city.PeopleId);

            if (result.Message == ActionMessages.Updated)
            {
                return RedirectToAction(nameof(Details), "City", new { id = result.City.Id, updated = true });
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "Could not find the requested city.");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message.ToString());

                return BadRequest(ModelState);
            }
        }

        #endregion AddPeople

        #region RemovePeople

        [HttpGet]
        public async Task<IActionResult> RemovePeople(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "The Id could not be fetched.");

                return BadRequest(ModelState);
            }

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            {
                return View(result.City);
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "Could not find the requested city.");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error type: {result.Message.ToString()}");

                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePeople([FromForm]RemovePeopleFromCity remove)
        {
            if (remove.CityId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "The Id could not be fetched.");

                return BadRequest(ModelState);
            }

            var result = await _service.RemovePeople(remove);

            if (result.Message == ActionMessages.Updated)
            {
                return RedirectToAction(nameof(Details), "City", new { id = result.City.Id, updated = true });
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "The requested city could not be found.");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error Type: {result.Message.ToString()}");

                return BadRequest(ModelState);
            }
        }

        #endregion RemovePeople

        #endregion Edit

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "The Id Could not be fetched.");

                return BadRequest(ModelState);
            }

            var result = await _service.Find(id);

            if (result.Message == ActionMessages.Success)
            {
                return View(result.City);
            }
            else if (result.Message == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "Could not find the requested city.");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error Type: {result.Message.ToString()}");

                return BadRequest(ModelState);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "The Id Could not be fetched.");

                return BadRequest(ModelState);
            }

            var result = await _service.Delete(id);

            if (result == ActionMessages.Deleted)
            {
                return RedirectToAction(nameof(Index), "City", new { message = result.ToString() });
            }
            else if (result == ActionMessages.NotFound)
            {
                ModelState.AddModelError(string.Empty, "The requested city was not found.");

                return NotFound(ModelState);
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error Type: {result.ToString()}");

                return BadRequest(ModelState);
            }
        }

        #endregion Delete
    }
}