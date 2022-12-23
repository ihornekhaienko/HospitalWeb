using AutoMapper;
using FluentAssertions;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Interfaces;
using HospitalWeb.Services.Extensions;
using HospitalWeb.Tests.Services;
using HospitalWeb.WebApi.Controllers;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace HospitalWeb.WebApi.Tests
{
    public class AppUsersControllerTests
    {
        [Fact]
        public async Task AppUsersControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            var appUsers = DataGenerator.GetTestAppUsers();

            var appUserRepo = new Mock<IRepository<AppUser>>();
            appUserRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<AppUser, bool>>(),
                It.IsAny<Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>>(),
                It.IsAny<Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(appUsers);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var response = await controller.Get();
            var result = response.Result as ObjectResult;
            var value = result?.Value as IEnumerable<AppUser>;

            //Assert
            appUserRepo.Verify();
            value.Should().NotBeNullOrEmpty();
            value.Should().BeOfType<List<AppUser>>();
            value.Should().AllBeOfType<AppUser>();
            value.Count().Should().Be(appUsers.Count);
        }

        [Fact]
        public async Task AppUsersControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            var appUsers = DataGenerator.GetTestAppUsers();
            string id = appUsers.Last().Id;
            var correct = appUsers.Where(a => a.Id == id).First();

            var appUserRepo = new Mock<IRepository<AppUser>>();
            appUserRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>(),
                It.IsAny<Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            appUserRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as AppUser;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task AppUsersControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            string id = Guid.NewGuid().ToString();
            var appUsers = DataGenerator.GetTestAppUsers();

            var appUserRepo = new Mock<IRepository<AppUser>>();
            appUserRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>(),
                It.IsAny<Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task AppUsersControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            var entity = DataGenerator.GetTestAppUsers().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<AppUser, AppUserResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<AppUser, AppUserResourceModel>(entity);

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<AppUser>())).ReturnsAsync(IdentityResult.Success);

            var appUserRepo = new Mock<IRepository<AppUser>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AppUsersControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            var entity = DataGenerator.GetTestAppUsers().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<AppUser, AppUserResourceModel>()
                .ForMember(d => d.Image, o => o.Ignore()));
            var mapper = new Mapper(config);
            var model = mapper.Map<AppUser, AppUserResourceModel>(entity);

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<AppUser>())).ReturnsAsync(IdentityResult.Success);

            var appUserRepo = new Mock<IRepository<AppUser>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            userManager.Verify(u => u.CreateAsync(It.IsAny<AppUser>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as AppUser;
            value.Should().NotBeNull();
            value.Should().BeOfType<AppUser>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<AppUserResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task AppUsersControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            var appUser = DataGenerator.GetTestAppUsers().First();

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<AppUser>())).ReturnsAsync(IdentityResult.Success);

            var appUserRepo = new Mock<IRepository<AppUser>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Put(appUser);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AppUsersControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            var appUser = DataGenerator.GetTestAppUsers().First();

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<AppUser>())).ReturnsAsync(IdentityResult.Success);

            var appUserRepo = new Mock<IRepository<AppUser>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Put(appUser);

            //Assert
            userManager.Verify(u => u.UpdateAsync(It.IsAny<AppUser>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as AppUser;
            value.Should().NotBeNull();
            value.Should().BeOfType<AppUser>();
            value?.Id.Should().Be(appUser.Id);
        }

        [Fact]
        public async Task AppUsersControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            var appUser = DataGenerator.GetTestAppUsers().First();

            var appUserRepo = new Mock<IRepository<AppUser>>();
            appUserRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>(),
                It.IsAny<Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>>())).ReturnsAsync(appUser).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Delete(appUser.Id);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AppUsersControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppUsersController>>();

            var appUser = DataGenerator.GetTestAppUsers().First();

            var appUserRepo = new Mock<IRepository<AppUser>>();
            appUserRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<AppUser, bool>>>(),
                It.IsAny<Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>>())).ReturnsAsync(appUser).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.AppUsers).Returns(appUserRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AppUsersController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Delete(appUser.Id);

            //Assert
            userManager.Verify(u => u.DeleteAsync(It.IsAny<AppUser>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as AppUser;
            value.Should().NotBeNull();
            value.Should().BeOfType<AppUser>();
            value.Should().BeEquivalentTo(appUser);
        }
    }
}
