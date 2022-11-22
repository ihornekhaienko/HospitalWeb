using AutoMapper;
using FluentAssertions;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.Tests.Services;
using HospitalWeb.WebApi.Controllers;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace HospitalWeb.WebApi.Tests
{
    public class PatientsControllerTests
    {
        [Fact]
        public async Task PatientsControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            var headerMock = new Mock<IHeaderDictionary>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Response.Headers).Returns(headerMock.Object);

            var patients = DataGenerator.GetTestPatients();

            var patientRepo = new Mock<IRepository<Patient>>();
            patientRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Patient, bool>>(),
                It.IsAny<Func<IQueryable<Patient>, IOrderedQueryable<Patient>>>(),
                It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(patients);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new PatientsController(logger, uow.Object, userManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            //Act
            var result = await controller.Get(string.Empty, null);

            //Assert
            patientRepo.Verify();
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<Patient>>();
            result.Should().AllBeOfType<Patient>();
            result.Count().Should().Be(patients.Count);
        }

        [Fact]
        public async Task PatientsControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            var patients = DataGenerator.GetTestPatients();
            string id = patients.Last().Id;
            var correct = patients.Where(a => a.Id == id).First();

            var patientRepo = new Mock<IRepository<Patient>>();
            patientRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Patient, bool>>>(),
                It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new PatientsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            patientRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Patient;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task PatientsControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            string id = Guid.NewGuid().ToString();
            var patients = DataGenerator.GetTestPatients();

            var patientRepo = new Mock<IRepository<Patient>>();
            patientRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Patient, bool>>>(),
                It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new PatientsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task PatientsControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            var entity = DataGenerator.GetTestPatients().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Patient, PatientResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Patient, PatientResourceModel>(entity);

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<Patient>())).ReturnsAsync(IdentityResult.Success);

            var patientRepo = new Mock<IRepository<Patient>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var controller = new PatientsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task PatientsControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            var entity = DataGenerator.GetTestPatients().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Patient, PatientResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Patient, PatientResourceModel>(entity);

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<Patient>())).ReturnsAsync(IdentityResult.Success);

            var patientRepo = new Mock<IRepository<Patient>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var controller = new PatientsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            userManager.Verify(u => u.CreateAsync(It.IsAny<Patient>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Patient;
            value.Should().NotBeNull();
            value.Should().BeOfType<Patient>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<PatientResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task PatientsControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            var patient = DataGenerator.GetTestPatients().First();

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<Patient>())).ReturnsAsync(IdentityResult.Success);

            var patientRepo = new Mock<IRepository<Patient>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var controller = new PatientsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Put(patient);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task PatientsControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            var patient = DataGenerator.GetTestPatients().First();

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<Patient>())).ReturnsAsync(IdentityResult.Success);

            var patientRepo = new Mock<IRepository<Patient>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var controller = new PatientsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Put(patient);

            //Assert
            userManager.Verify(u => u.UpdateAsync(It.IsAny<Patient>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Patient;
            value.Should().NotBeNull();
            value.Should().BeOfType<Patient>();
            value?.Id.Should().Be(patient.Id);
        }

        [Fact]
        public async Task PatientsControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            var patient = DataGenerator.GetTestPatients().First();

            var patientRepo = new Mock<IRepository<Patient>>();
            patientRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Patient, bool>>>(),
                It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>())).ReturnsAsync(patient).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new PatientsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Delete(patient.Id);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task PatientsControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<PatientsController>>();

            var patient = DataGenerator.GetTestPatients().First();

            var patientRepo = new Mock<IRepository<Patient>>();
            patientRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Patient, bool>>>(),
                It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>())).ReturnsAsync(patient).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Patients).Returns(patientRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new PatientsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Delete(patient.Id);

            //Assert
            userManager.Verify(u => u.DeleteAsync(It.IsAny<Patient>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Patient;
            value.Should().NotBeNull();
            value.Should().BeOfType<Patient>();
            value.Should().BeEquivalentTo(patient);
        }
    }
}
