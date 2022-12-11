using AutoMapper;
using FluentAssertions;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.Services.Extensions;
using HospitalWeb.WebApi.Controllers;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace HospitalWeb.WebApi.Tests
{
    public class LocalitiesControllerTests
    {
        [Fact]
        public async Task LocalitiesControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            var localities = DataGenerator.GetTestLocalities();

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            diagnosisRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Locality, bool>>(),
                It.IsAny<Func<IQueryable<Locality>, IOrderedQueryable<Locality>>>(),
                It.IsAny<Func<IQueryable<Locality>, IIncludableQueryable<Locality, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(localities);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var result = await controller.Get();

            //Assert
            diagnosisRepo.Verify();
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<Locality>>();
            result.Should().AllBeOfType<Locality>();
            result.Count().Should().Be(localities.Count);
        }

        [Fact]
        public async Task LocalitiesControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            int id = 5;
            var localities = DataGenerator.GetTestLocalities();
            var correct = localities.Where(a => a.LocalityId == id).First();

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Locality, bool>>>(),
                It.IsAny<Func<IQueryable<Locality>, IIncludableQueryable<Locality, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(5);

            //Assert
            diagnosisRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Locality;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task LocalitiesControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            int id = 1000;
            var localities = DataGenerator.GetTestLocalities();

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Locality, bool>>>(),
                It.IsAny<Func<IQueryable<Locality>, IIncludableQueryable<Locality, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task LocalitiesControllerGetByName_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            int id = 5;
            var localities = DataGenerator.GetTestLocalities();
            var correct = localities.Where(a => a.LocalityId == id).First();
            string name = correct.LocalityName;

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Locality, bool>>>(),
                It.IsAny<Func<IQueryable<Locality>, IIncludableQueryable<Locality, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(name);

            //Assert
            diagnosisRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Locality;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task LocalitiesControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            var entity = DataGenerator.GetTestLocalities().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Locality, LocalityResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Locality, LocalityResourceModel>(entity);

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task LocalitiesControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            var entity = DataGenerator.GetTestLocalities().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Locality, LocalityResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Locality, LocalityResourceModel>(entity);

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            diagnosisRepo.Verify(r => r.CreateAsync(It.IsAny<Locality>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Locality;
            value.Should().NotBeNull();
            value.Should().BeOfType<Locality>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<LocalityResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task LocalitiesControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            var diagnosis = DataGenerator.GetTestLocalities().First();

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(diagnosis);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task LocalitiesControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            var diagnosis = DataGenerator.GetTestLocalities().First();

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(diagnosis);

            //Assert
            diagnosisRepo.Verify(r => r.UpdateAsync(It.IsAny<Locality>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Locality;
            value.Should().NotBeNull();
            value.Should().BeOfType<Locality>();
            value?.LocalityId.Should().Be(diagnosis.LocalityId);
        }

        [Fact]
        public async Task LocalitiesControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            var diagnosis = DataGenerator.GetTestLocalities().First();

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Locality, bool>>>(),
                It.IsAny<Func<IQueryable<Locality>, IIncludableQueryable<Locality, object>>>())).ReturnsAsync(diagnosis).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(diagnosis.LocalityId);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task LocalitiesControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<LocalitiesController>>();

            var diagnosis = DataGenerator.GetTestLocalities().First();

            var diagnosisRepo = new Mock<IRepository<Locality>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Locality, bool>>>(),
                It.IsAny<Func<IQueryable<Locality>, IIncludableQueryable<Locality, object>>>())).ReturnsAsync(diagnosis).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Localities).Returns(diagnosisRepo.Object);

            var controller = new LocalitiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(diagnosis.LocalityId);

            //Assert
            diagnosisRepo.Verify(r => r.DeleteAsync(It.IsAny<Locality>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Locality;
            value.Should().NotBeNull();
            value.Should().BeOfType<Locality>();
            value.Should().BeEquivalentTo(diagnosis);
        }
    }
}
