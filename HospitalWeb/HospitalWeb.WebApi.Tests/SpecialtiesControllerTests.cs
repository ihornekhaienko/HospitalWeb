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
    public class SpecialtiesControllerTests
    {
        [Fact]
        public async Task SpecialtiesControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            var specialties = DataGenerator.GetTestSpecialties();

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            diagnosisRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Specialty, bool>>(),
                It.IsAny<Func<IQueryable<Specialty>, IOrderedQueryable<Specialty>>>(),
                It.IsAny<Func<IQueryable<Specialty>, IIncludableQueryable<Specialty, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(specialties);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var response = await controller.Get();
            var result = response.Result as ObjectResult;
            var value = result?.Value as IEnumerable<Specialty>;

            //Assert
            diagnosisRepo.Verify();
            value.Should().NotBeNullOrEmpty();
            value.Should().BeOfType<List<Specialty>>();
            value.Should().AllBeOfType<Specialty>();
            value.Count().Should().Be(specialties.Count);
        }

        [Fact]
        public async Task SpecialtiesControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            int id = 5;
            var specialties = DataGenerator.GetTestSpecialtiesWithId();
            var correct = specialties.Where(a => a.SpecialtyId == id).First();

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Specialty, bool>>>(),
                It.IsAny<Func<IQueryable<Specialty>, IIncludableQueryable<Specialty, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(5);

            //Assert
            diagnosisRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Specialty;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task SpecialtiesControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            int id = 1000;
            var specialties = DataGenerator.GetTestSpecialties();

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Specialty, bool>>>(),
                It.IsAny<Func<IQueryable<Specialty>, IIncludableQueryable<Specialty, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task SpecialtiesControllerGetByName_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            int id = 5;
            var specialties = DataGenerator.GetTestSpecialtiesWithId();
            var correct = specialties.Where(a => a.SpecialtyId == id).First();
            string name = correct.SpecialtyName;

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Specialty, bool>>>(),
                It.IsAny<Func<IQueryable<Specialty>, IIncludableQueryable<Specialty, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(name);

            //Assert
            diagnosisRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Specialty;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task SpecialtiesControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            var entity = DataGenerator.GetTestSpecialties().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Specialty, SpecialtyResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Specialty, SpecialtyResourceModel>(entity);

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task SpecialtiesControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            var entity = DataGenerator.GetTestSpecialties().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Specialty, SpecialtyResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Specialty, SpecialtyResourceModel>(entity);

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            diagnosisRepo.Verify(r => r.CreateAsync(It.IsAny<Specialty>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Specialty;
            value.Should().NotBeNull();
            value.Should().BeOfType<Specialty>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<SpecialtyResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task SpecialtiesControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            var diagnosis = DataGenerator.GetTestSpecialties().First();

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(diagnosis);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task SpecialtiesControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            var diagnosis = DataGenerator.GetTestSpecialties().First();

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(diagnosis);

            //Assert
            diagnosisRepo.Verify(r => r.UpdateAsync(It.IsAny<Specialty>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Specialty;
            value.Should().NotBeNull();
            value.Should().BeOfType<Specialty>();
            value?.SpecialtyId.Should().Be(diagnosis.SpecialtyId);
        }

        [Fact]
        public async Task SpecialtiesControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            var diagnosis = DataGenerator.GetTestSpecialties().First();

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Specialty, bool>>>(),
                It.IsAny<Func<IQueryable<Specialty>, IIncludableQueryable<Specialty, object>>>())).ReturnsAsync(diagnosis).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(diagnosis.SpecialtyId);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task SpecialtiesControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SpecialtiesController>>();

            var diagnosis = DataGenerator.GetTestSpecialties().First();

            var diagnosisRepo = new Mock<IRepository<Specialty>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Specialty, bool>>>(),
                It.IsAny<Func<IQueryable<Specialty>, IIncludableQueryable<Specialty, object>>>())).ReturnsAsync(diagnosis).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Specialties).Returns(diagnosisRepo.Object);

            var controller = new SpecialtiesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(diagnosis.SpecialtyId);

            //Assert
            diagnosisRepo.Verify(r => r.DeleteAsync(It.IsAny<Specialty>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Specialty;
            value.Should().NotBeNull();
            value.Should().BeOfType<Specialty>();
            value.Should().BeEquivalentTo(diagnosis);
        }
    }
}
