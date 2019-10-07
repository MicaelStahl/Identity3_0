using BusinessLibrary.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BusinessLibraryTesting
{
    public class PersonRepositoryTesting
    {
        #region D.I

        private readonly Mock<IPersonRepository> _service;

        public PersonRepositoryTesting()
        {
            _service = new Mock<IPersonRepository>();
        }

        #endregion D.I

        #region References

        /// <summary>
        /// Creates a new Id
        /// </summary>
        private Guid IdGeneration()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Creates one person with valid data.
        /// </summary>
        private Person OneValidPerson()
        {
            return new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Testsson", Age = 25, Email = "Test.Testsson@context.com", PhoneNumber = "123456789" };
        }

        /// <summary>
        /// Creates one person with invalid data.
        /// </summary>
        private Person OneInvalidPerson()
        {
            return new Person { Id = Guid.NewGuid(), FirstName = "", LastName = "", Age = 22, Email = "", PhoneNumber = "" };
        }

        /// <summary>
        /// Creates a list of two valid people.
        /// </summary>
        private List<Person> TwoValidPeople()
        {
            return new List<Person>
            {
                new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Testsson", Age = 22, Email = "Test.Testsson@context.com", PhoneNumber = "123456789" },
                new Person { Id = Guid.NewGuid(), FirstName = "Test2", LastName = "TestssoN", Age = 33, Email = "Test2.Testsson@context.com", PhoneNumber = "123456789" }
            };
        }

        #endregion References

        #region Create

        [Fact]
        [Trait("Repository", "Person-Create")]
        public async Task Create_SubmitValidData_ReturnsActionMessageIndicatingCreatedAsync()
        {
            var person = OneValidPerson();
            _service.Setup(x => x.Create(person, Guid.Empty)).ReturnsAsync(ActionMessages.Created);

            Assert.Equal(ActionMessages.Created, await _service.Object.Create(person, Guid.Empty));
        }

        [Fact]
        [Trait("Repository", "Person-Create")]
        public async Task Create_SubmitInvalidData_ReturnsActionMessageIndicatingNotAllFieldsWereFilledAsync()
        {
            var person = OneInvalidPerson();
            _service.Setup(x => x.Create(person, Guid.Empty)).Returns(Task.FromResult(ActionMessages.FillAllFields));

            Assert.Equal(ActionMessages.FillAllFields, await _service.Object.Create(person, Guid.Empty));
        }

        [Fact]
        [Trait("Repository", "Person-Create")]
        public async Task Create_SubmitInvalidAge_ReturnsActionMessageIndicatingInvalidAgeAsync()
        {
            var person = OneValidPerson();
            person.Age = 200;
            _service.Setup(x => x.Create(person, Guid.Empty)).Returns(Task.FromResult(ActionMessages.InvalidAge));

            Assert.Equal(ActionMessages.InvalidAge, await _service.Object.Create(person, Guid.Empty));
        }

        #endregion Create

        #region Find

        #region FindOne

        [Fact]
        [Trait("Repository", "Person-FindOne")]
        public async Task Find_SubmitValidId_ReturnsCorrectPersonAsync()
        {
            var person = OneValidPerson();
            _service.Setup(x => x.Find(person.Id)).ReturnsAsync(new PersonWithMessage { Person = person, Message = ActionMessages.Success });

            var result = await _service.Object.Find(person.Id);

            Assert.Equal(person, result.Person);
        }

        [Fact]
        [Trait("Repository", "Person-FindOne")]
        public async Task Find_SubmitInvalidId_ReturnsActionMessageIndicatingNotFoundAsync()
        {
            var fakeId = IdGeneration();
            _service.Setup(x => x.Find(fakeId)).Returns(Task.FromResult(new PersonWithMessage { Message = ActionMessages.NotFound }));

            var result = await _service.Object.Find(fakeId);

            Assert.Equal(ActionMessages.NotFound, result.Message);
        }

        [Fact]
        [Trait("Repository", "Person-FindOne")]
        public async Task Find_SubmitEmptyId_ReturnsActionMessageIndicatingFailedAsync()
        {
            _service.Setup(x => x.Find(Guid.Empty)).Returns(Task.FromResult(new PersonWithMessage { Message = ActionMessages.Failed }));

            var result = await _service.Object.Find(Guid.Empty);

            Assert.Equal(ActionMessages.Failed, result.Message);
        }

        [Fact]
        [Trait("Repository", "Person-FindOne")]
        public async Task Find_SubmitValidId_ThrowsExceptionIfTwoPeopleWithSameIdExistsAsync()
        {
            var person = OneValidPerson();
            // This indicates that there already exists more than one person in database with this ID.
            _service.Setup(x => x.Find(person.Id)).ThrowsAsync(new Exception());

            // And this verifies that claim.
            await Assert.ThrowsAsync<Exception>(() => _service.Object.Find(person.Id));
        }

        #endregion FindOne

        #region FindAll

        [Fact]
        [Trait("Repository", "Person-FindAll")]
        public async Task Find_CallMethod_ReturnsListOfTwoAsync()
        {
            var people = TwoValidPeople();
            var peopleList = new PersonListWithMessage { Message = ActionMessages.Success, People = people };
            _service.Setup(x => x.FindAll()).ReturnsAsync(peopleList);

            Assert.Equal(peopleList, await _service.Object.FindAll());
        }

        [Fact]
        [Trait("Repository", "Person-FindAll")]
        public async Task Find_CallMethod_ReturnsActionMessageIndicatingEmptyIfListIsEmptyAsync()
        {
            _service.Setup(x => x.FindAll()).Returns(Task.FromResult(new PersonListWithMessage { Message = ActionMessages.Empty }));

            var result = await _service.Object.FindAll();

            Assert.Equal(ActionMessages.Empty, result.Message);
        }

        #endregion FindAll

        #endregion Find

        #region Edit

        [Fact]
        [Trait("Repository", "Person-Edit")]
        public async Task Edit_SubmitValidData_ReturnsActionMessageIndicatingUpdatedAsync() // Might be invalid in the future.
        {
            var person = OneValidPerson();
            // This indicates a person already exists with the given ID and is now being updated with the given data.
            _service.Setup(x => x.Edit(person, Guid.Empty)).ReturnsAsync(ActionMessages.Updated);

            // And this verifies that claim.
            Assert.Equal(ActionMessages.Updated, await _service.Object.Edit(person, Guid.Empty));
        }

        [Fact]
        [Trait("Repository", "Person-Edit")]
        public async Task Edit_SubmitInvalidData_ReturnsActionMessageIndicatingNotAllFieldsWereFilledAsync()
        {
            var person = OneInvalidPerson();
            // This indicates a person already exists with the given ID and is now being updated with the given data.
            _service.Setup(x => x.Edit(person, Guid.Empty)).Returns(Task.FromResult(ActionMessages.FillAllFields));

            // And this verifies that claim.
            Assert.Equal(ActionMessages.FillAllFields, await _service.Object.Edit(person, Guid.Empty));
        }

        [Fact]
        [Trait("Repository", "Person-Edit")]
        public async Task Edit_SubmitInvalidData_ReturnsNonUpdatedPersonAsync()
        {
            var person = OneValidPerson();
            var personEdit = person;
            personEdit.FirstName = "";
            _service.Setup(x => x.Edit(personEdit, Guid.Empty));
            _service.Setup(x => x.Find(personEdit.Id)).ReturnsAsync(new PersonWithMessage { Person = person, Message = ActionMessages.Success });

            var result = await _service.Object.Find(personEdit.Id);

            Assert.Equal(person, result.Person);
        }

        #endregion Edit

        #region Delete

        [Fact]
        [Trait("Repository", "Person-Delete")]
        public async Task Delete_SubmitValidId_ReturnsActionMessageIndicatingDeletedAsync()
        {
            var personId = IdGeneration();
            _service.Setup(x => x.Delete(personId)).ReturnsAsync(ActionMessages.Deleted);

            Assert.Equal(ActionMessages.Deleted, await _service.Object.Delete(personId));
        }

        [Fact]
        [Trait("Repository", "Person-Delete")]
        public async Task Delete_SubmitValidId_ReturnsListWithoutDeletedPersonAsync()
        {
            var people = TwoValidPeople();
            // To easier verify that the correct one is removed.
            var person = people.LastOrDefault();
            _service.Setup(x => x.Delete(person.Id)).ReturnsAsync(ActionMessages.Deleted);
            _service.Setup(x => x.FindAll()).ReturnsAsync(new PersonListWithMessage { People = people.TakeWhile(x => x.Id != person.Id).ToList(), Message = ActionMessages.Success });

            var result = await _service.Object.FindAll();

            // Verifies that no element in the sequence is equal to the element that was removed.
            result.People.ForEach(x => Assert.NotEqual(person, x));
        }

        [Fact]
        [Trait("Repository", "Person-Delete")]
        public async Task Delete_SubmitInvalidId_ReturnsActionMessageIndicatingNotFoundAsync()
        {
            var fakeId = IdGeneration();
            _service.Setup(x => x.Delete(fakeId)).Returns(Task.FromResult(ActionMessages.NotFound));

            Assert.Equal(ActionMessages.NotFound, await _service.Object.Delete(fakeId));
        }

        [Fact]
        [Trait("Repository", "Person-Delete")]
        public async Task Delete_SubmitEmptyGuid_ThrowsExceptionAsync()
        {
            _service.Setup(x => x.Delete(Guid.Empty)).ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _service.Object.Delete(Guid.Empty));
        }

        #endregion Delete
    }
}