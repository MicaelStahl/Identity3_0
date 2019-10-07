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

        private Person OneValidPerson()
        {
            return new Person { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Testsson", Age = 25, Email = "Test.Testsson@context.com", PhoneNumber = "123456789" };
        }

        private Person OneInvalidPerson()
        {
            return new Person { Id = Guid.NewGuid(), FirstName = "", LastName = "", Age = 22, Email = "", PhoneNumber = "" };
        }

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

        [Fact]
        [Trait("Repository", "Person-Create")]
        public async Task Create_SubmitValidData_ThrowsExceptionAndReturnsFailedActionMessageAsync()
        {
            var person = OneValidPerson();
            _service.Setup(x => x.Create(person, Guid.Empty)).ThrowsAsync(new Exception());

            var result = await Assert.ThrowsAsync<Exception>(() => _service.Object.Create(person, Guid.Empty));
            Assert.Equal(ActionMessages.Failed.ToString(), result.Message);
        }

        #endregion Create
    }
}