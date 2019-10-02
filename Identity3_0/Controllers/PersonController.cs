using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BusinessLibrary.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Identity3_0.Controllers
{
    [Route("[controller]/[action]")]
    public class PersonController : Controller
    {
        private readonly IPersonRepository _service;
        private readonly IGlobalRepository _list;
        private readonly DictionaryMessages _dictionary;

        public PersonController(IPersonRepository service, IGlobalRepository list)
        {
            _service = service;
            _list = list;
            _dictionary = new DictionaryMessages();
        }

        /// <summary>
        /// Converts an enum object to it's string representative.
        /// </summary>
        protected string ConvertEnumToString(ActionMessages message)
        {
            return _dictionary.Messages.FirstOrDefault(x => x.Key == (int)message).Value.Replace('_', ' ');
            //return test.Value;
            //return Enum.GetName(typeof(ActionMessages), message);
        }

        public async Task<IActionResult> Index(string message = null) // if nothing else is stated it gets the value of Success.
        {
            if (!string.IsNullOrWhiteSpace(message))
                ViewBag.message = $"The requested person was successfully {message.ToLower()}!";

            return View(await _list.GetPeople());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new PersonUpdate { Cities = await _list.GetCities(), Person = new Person() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person, Guid? CityId)
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
                    return RedirectToAction(nameof(Index), "Person", new { message = ConvertEnumToString(result) });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Person person, Guid? CityId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Please check all fields and try again.");
                }

                var result = await _service.Edit(person, CityId);

                if (result == ActionMessages.Updated)
                {
                    return RedirectToAction(nameof(Index), "Person", new { message = ConvertEnumToString(result) });
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
                    ModelState.AddModelError(string.Empty, "Something went wrong.");
                    throw new Exception(ModelState.ValidationState.ToString());
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
                    return RedirectToAction(nameof(Index), "Person", new { message = ConvertEnumToString(result) });
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