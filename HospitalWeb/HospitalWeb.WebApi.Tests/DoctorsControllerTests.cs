using AutoMapper;
using FluentAssertions;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.WebApi.Services.Extensions;
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
    public class DoctorsControllerTests
    {
        [Fact]
        public async Task DoctorsControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            var headerMock = new Mock<IHeaderDictionary>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Response.Headers).Returns(headerMock.Object);

            var doctors = DataGenerator.GetTestDoctors();

            var doctorRepo = new Mock<IRepository<Doctor>>();
            doctorRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Doctor, bool>>(),
                It.IsAny<Func<IQueryable<Doctor>, IOrderedQueryable<Doctor>>>(),
                It.IsAny<Func<IQueryable<Doctor>, IIncludableQueryable<Doctor, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(doctors);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new DoctorsController(logger, uow.Object, userManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            //Act
            var response = await controller.Get(string.Empty, null);
            var result = response.Result as ObjectResult;
            var value = result?.Value as IEnumerable<Doctor>;

            //Assert
            doctorRepo.Verify();
            value.Should().NotBeNullOrEmpty();
            value.Should().BeOfType<List<Doctor>>();
            value.Should().AllBeOfType<Doctor>();
            value.Count().Should().Be(doctors.Count);
        }

        [Fact]
        public async Task DoctorsControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            var doctors = DataGenerator.GetTestDoctors();
            string id = doctors.Last().Id;
            var correct = doctors.Where(a => a.Id == id).First();

            var doctorRepo = new Mock<IRepository<Doctor>>();
            doctorRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Doctor, bool>>>(),
                It.IsAny<Func<IQueryable<Doctor>, IIncludableQueryable<Doctor, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new DoctorsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            doctorRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Doctor;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task DoctorsControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            string id = Guid.NewGuid().ToString();
            var doctors = DataGenerator.GetTestDoctors();

            var doctorRepo = new Mock<IRepository<Doctor>>();
            doctorRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Doctor, bool>>>(),
                It.IsAny<Func<IQueryable<Doctor>, IIncludableQueryable<Doctor, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new DoctorsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task DoctorsControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            var entity = DataGenerator.GetTestDoctors().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Doctor, DoctorResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Doctor, DoctorResourceModel>(entity);

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<Doctor>())).ReturnsAsync(IdentityResult.Success);

            var doctorRepo = new Mock<IRepository<Doctor>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var controller = new DoctorsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task DoctorsControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            var entity = DataGenerator.GetTestDoctors().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Doctor, DoctorResourceModel>()
                .ForMember(d => d.Image, o => o.Ignore()));
            var mapper = new Mapper(config);
            var model = mapper.Map<Doctor, DoctorResourceModel>(entity);

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<Doctor>())).ReturnsAsync(IdentityResult.Success);

            var doctorRepo = new Mock<IRepository<Doctor>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var controller = new DoctorsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            userManager.Verify(u => u.CreateAsync(It.IsAny<Doctor>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Doctor;
            value.Should().NotBeNull();
            value.Should().BeOfType<Doctor>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<DoctorResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task DoctorsControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            var doctor = DataGenerator.GetTestDoctors().First();

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<Doctor>())).ReturnsAsync(IdentityResult.Success);

            var doctorRepo = new Mock<IRepository<Doctor>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var controller = new DoctorsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Put(doctor);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task DoctorsControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            var doctor = DataGenerator.GetTestDoctors().First();

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<Doctor>())).ReturnsAsync(IdentityResult.Success);

            var doctorRepo = new Mock<IRepository<Doctor>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var controller = new DoctorsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Put(doctor);

            //Assert
            userManager.Verify(u => u.UpdateAsync(It.IsAny<Doctor>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Doctor;
            value.Should().NotBeNull();
            value.Should().BeOfType<Doctor>();
            value?.Id.Should().Be(doctor.Id);
        }

        [Fact]
        public async Task DoctorsControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            var doctor = DataGenerator.GetTestDoctors().First();

            var doctorRepo = new Mock<IRepository<Doctor>>();
            doctorRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Doctor, bool>>>(),
                It.IsAny<Func<IQueryable<Doctor>, IIncludableQueryable<Doctor, object>>>())).ReturnsAsync(doctor).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new DoctorsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Delete(doctor.Id);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task DoctorsControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DoctorsController>>();

            var doctor = DataGenerator.GetTestDoctors().First();

            var doctorRepo = new Mock<IRepository<Doctor>>();
            doctorRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Doctor, bool>>>(),
                It.IsAny<Func<IQueryable<Doctor>, IIncludableQueryable<Doctor, object>>>())).ReturnsAsync(doctor).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Doctors).Returns(doctorRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new DoctorsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Delete(doctor.Id);

            //Assert
            userManager.Verify(u => u.DeleteAsync(It.IsAny<Doctor>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Doctor;
            value.Should().NotBeNull();
            value.Should().BeOfType<Doctor>();
            value.Should().BeEquivalentTo(doctor);
        }
    }
}
