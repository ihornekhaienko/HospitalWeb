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
    public class AdminsControllerTests
    {
        [Fact]
        public async Task AdminsControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var headerMock = new Mock<IHeaderDictionary>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Response.Headers).Returns(headerMock.Object);

            var admins = DataGenerator.GetTestAdmins();

            var adminRepo = new Mock<IRepository<Admin>>();
            adminRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Admin, bool>>(),
                It.IsAny<Func<IQueryable<Admin>, IOrderedQueryable<Admin>>>(),
                It.IsAny<Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(admins);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AdminsController(logger, uow.Object, userManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            //Act
            var response = await controller.Get();
            var result = response.Result as ObjectResult;
            var value = result?.Value as IEnumerable<Admin>;

            //Assert
            adminRepo.Verify();
            value.Should().NotBeNullOrEmpty();
            value.Should().BeOfType<List<Admin>>();
            value.Should().AllBeOfType<Admin>();
            value.Count().Should().Be(admins.Count);
        }

        [Fact]
        public async Task AdminsControllerSearch_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var headerMock = new Mock<IHeaderDictionary>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Response.Headers).Returns(headerMock.Object);

            var admins = DataGenerator.GetTestAdmins();

            var searchString = admins.Last().Email;
            int pageSize = 10;

            var filtered = admins.Where(a => a.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).Take(pageSize).ToList();

            var adminRepo = new Mock<IRepository<Admin>>();
            adminRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Admin, bool>>(),
                It.IsAny<Func<IQueryable<Admin>, IOrderedQueryable<Admin>>>(),
                It.IsAny<Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(filtered);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AdminsController(logger, uow.Object, userManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            //Act
            var response = await controller.Get(searchString: searchString, pageSize: pageSize);
            var result = response.Result as ObjectResult;
            var value = result?.Value as IEnumerable<Admin>;

            //Assert
            adminRepo.Verify();
            value.Should().NotBeNullOrEmpty();
            value.Should().BeOfType<List<Admin>>();
            value.Should().AllBeOfType<Admin>();
            value.Count().Should().Be(filtered.Count());
        }

        [Fact]
        public async Task AdminsControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var admins = DataGenerator.GetTestAdmins();
            string id = admins.Last().Id;
            var correct = admins.Where(a => a.Id == id).First();

            var adminRepo = new Mock<IRepository<Admin>>();
            adminRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Admin, bool>>>(),
                It.IsAny<Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AdminsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            adminRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Admin;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task AdminsControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            string id = Guid.NewGuid().ToString();
            var admins = DataGenerator.GetTestAdmins();

            var adminRepo = new Mock<IRepository<Admin>>();
            adminRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Admin, bool>>>(),
                It.IsAny<Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AdminsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task AdminsControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var entity = DataGenerator.GetTestAdmins().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Admin, AdminResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Admin, AdminResourceModel>(entity);

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<Admin>())).ReturnsAsync(IdentityResult.Success);

            var adminRepo = new Mock<IRepository<Admin>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var controller = new AdminsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AdminsControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var entity = DataGenerator.GetTestAdmins().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Admin, AdminResourceModel>()
                .ForMember(d => d.Image, o => o.Ignore()));
            var mapper = new Mapper(config);
            var model = mapper.Map<Admin, AdminResourceModel>(entity);

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<Admin>())).ReturnsAsync(IdentityResult.Success);

            var adminRepo = new Mock<IRepository<Admin>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var controller = new AdminsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            userManager.Verify(u => u.CreateAsync(It.IsAny<Admin>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Admin;
            value.Should().NotBeNull();
            value.Should().BeOfType<Admin>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<AdminResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task AdminsControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var admin = DataGenerator.GetTestAdmins().First();

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<Admin>())).ReturnsAsync(IdentityResult.Success);

            var adminRepo = new Mock<IRepository<Admin>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var controller = new AdminsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Put(admin);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AdminsControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var admin = DataGenerator.GetTestAdmins().First();

            var userManager = MockHelpers.MockUserManager<AppUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<Admin>())).ReturnsAsync(IdentityResult.Success);

            var adminRepo = new Mock<IRepository<Admin>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var controller = new AdminsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Put(admin);

            //Assert
            userManager.Verify(u => u.UpdateAsync(It.IsAny<Admin>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Admin;
            value.Should().NotBeNull();
            value.Should().BeOfType<Admin>();
            value?.Id.Should().Be(admin.Id);
        }

        [Fact]
        public async Task AdminsControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var admin = DataGenerator.GetTestAdmins().First();

            var adminRepo = new Mock<IRepository<Admin>>();
            adminRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Admin, bool>>>(),
                It.IsAny<Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>>())).ReturnsAsync(admin).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AdminsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Delete(admin.Id);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AdminsControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AdminsController>>();

            var admin = DataGenerator.GetTestAdmins().First();

            var adminRepo = new Mock<IRepository<Admin>>();
            adminRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Admin, bool>>>(),
                It.IsAny<Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>>())).ReturnsAsync(admin).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Admins).Returns(adminRepo.Object);

            var userManager = MockHelpers.MockUserManager<AppUser>();

            var controller = new AdminsController(logger, uow.Object, userManager.Object);

            //Act
            var actionResult = await controller.Delete(admin.Id);

            //Assert
            userManager.Verify(u => u.DeleteAsync(It.IsAny<Admin>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Admin;
            value.Should().NotBeNull();
            value.Should().BeOfType<Admin>();
            value.Should().BeEquivalentTo(admin);
        }
    }
}
