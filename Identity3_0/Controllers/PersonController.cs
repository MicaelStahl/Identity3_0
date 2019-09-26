using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity3_0.Interfaces;
using Identity3_0.Models;
using Identity3_0.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Identity3_0.Controllers
{
    [Route("[controller]/[action]")]
    public class PersonController : Controller
    {
        private readonly IPersonRepository _service;
        private readonly IGlobalRepository _list;

        public PersonController(IPersonRepository service, IGlobalRepository list)
        {
            _service = service;
            _list = list;
        }

        /// <summary>
        /// Converts an enum object to it's string representative.
        /// </summary>
        protected string ConvertEnumToString(ActionMessages message)
        {
            return Enum.GetName(typeof(ActionMessages), message);
        }

        public async Task<IActionResult> Index(string message = null) // if nothing else is stated it gets the value of Success.
        {
            // Using discard "_" to create a simple lambda expression depending what the message variable is.
            //_ = message == ActionMessages.Success ? null : ViewBag.message = ConvertEnumToString(message);
            if (!string.IsNullOrWhiteSpace(message))
                ViewBag.message = message;

            return View(await _list.GetPeople());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new PersonUpdate { Cities = await _list.GetCities(), Person = new Person() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person, Guid CityId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, ConvertEnumToString(ActionMessages.FillAllFields));
                    throw new ArgumentNullException();
                }

                var result = await _service.Create(person, CityId);

                if (result == ActionMessages.Created)
                {
                    return RedirectToAction(nameof(Index), "Person", new { message = ConvertEnumToString(ActionMessages.Created) });
                }
                else if (result == ActionMessages.NotFound)
                {
                    ModelState.AddModelError(string.Empty, ConvertEnumToString(ActionMessages.NotFound));

                    return NotFound(ModelState);
                }
                else
                {
                    throw new Exception(Enum.GetName(typeof(ActionMessages), result));
                }
            }
            catch (ArgumentNullException) // Catches the exception thrown if the modelstate was invalid.
            {
                return BadRequest(ModelState);
            }
            catch (Exception ex) // Catches all other exceptions thrown.
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Something went wrong.");
                }

                var result = await _service.Find(id);

                if (result.Message == ActionMessages.Success)
                {
                    return View(result.Person);
                }
                else if (result.Message == ActionMessages.NotFound)
                {
                    ModelState.AddModelError(string.Empty, ConvertEnumToString(result.Message));

                    return NotFound(ModelState);
                }
                else
                {
                    throw new Exception(ConvertEnumToString(result.Message));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Something went wrong.");
                }

                var result = await _service.Find(id);

                if (result.Message == ActionMessages.Success)
                {
                    return View(new PersonUpdate { Person = result.Person, Cities = await _list.GetCities() });
                }
                else if (result.Message == ActionMessages.NotFound)
                {
                    ModelState.AddModelError(string.Empty, ConvertEnumToString(result.Message));

                    return NotFound(ModelState);
                }
                else
                {
                    throw new Exception(ConvertEnumToString(result.Message));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(Person person)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Please check all fields and try again.");
                }

                var result = await _service.Edit(person);

                if (result == ActionMessages.Updated)
                {
                    return RedirectToAction(nameof(Index), new { message = ConvertEnumToString(result) });
                }
                else if (result == ActionMessages.NotFound)
                {
                    ModelState.AddModelError(string.Empty, ConvertEnumToString(result));

                    return NotFound(ModelState);
                }
                else
                {
                    throw new Exception(ConvertEnumToString(result));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Something went wrong.");
                }

                var result = await _service.Find(id);

                if (result.Message == ActionMessages.Success)
                {
                    return View(result.Person);
                }
                else if (result.Message == ActionMessages.NotFound)
                {
                    ModelState.AddModelError(string.Empty, ConvertEnumToString(result.Message));

                    return NotFound(ModelState);
                }
                else
                {
                    throw new Exception(ConvertEnumToString(result.Message));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new Exception("Something went wrong.");
                }

                var result = await _service.Delete(id);

                if (result == ActionMessages.Deleted)
                {
                    return RedirectToAction(nameof(Index), new { message = ConvertEnumToString(result) });
                }
                else if (result == ActionMessages.NotFound)
                {
                    ModelState.AddModelError(string.Empty, ConvertEnumToString(result));

                    return NotFound(ModelState);
                }
                else
                {
                    throw new Exception(ConvertEnumToString(result));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}