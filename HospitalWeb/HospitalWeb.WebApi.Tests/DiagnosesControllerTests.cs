using AutoMapper;
using FluentAssertions;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.Tests.Services;
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
    public class DiagnosesControllerTests
    {
        [Fact]
        public async Task DiagnosesControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            var diagnoses = DataGenerator.GetTestDiagnoses();

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            diagnosisRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Diagnosis, bool>>(),
                It.IsAny<Func<IQueryable<Diagnosis>, IOrderedQueryable<Diagnosis>>>(),
                It.IsAny<Func<IQueryable<Diagnosis>, IIncludableQueryable<Diagnosis, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(diagnoses);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var result = await controller.Get();

            //Assert
            diagnosisRepo.Verify();
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<Diagnosis>>();
            result.Should().AllBeOfType<Diagnosis>();
            result.Count().Should().Be(diagnoses.Count);
        }

        [Fact]
        public async Task DiagnosesControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            int id = 5;
            var diagnoses = DataGenerator.GetTestDiagnoses();
            var correct = diagnoses.Where(a => a.DiagnosisId == id).First();

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>(),
                It.IsAny<Func<IQueryable<Diagnosis>, IIncludableQueryable<Diagnosis, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(5);

            //Assert
            diagnosisRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Diagnosis;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task DiagnosesControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            int id = 1000;
            var diagnoses = DataGenerator.GetTestDiagnoses();

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>(),
                It.IsAny<Func<IQueryable<Diagnosis>, IIncludableQueryable<Diagnosis, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task DiagnosesControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            var entity = DataGenerator.GetTestDiagnoses().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Diagnosis, DiagnosisResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Diagnosis, DiagnosisResourceModel>(entity);

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task DiagnosesControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            var entity = DataGenerator.GetTestDiagnoses().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Diagnosis, DiagnosisResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Diagnosis, DiagnosisResourceModel>(entity);

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            diagnosisRepo.Verify(r => r.CreateAsync(It.IsAny<Diagnosis>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Diagnosis;
            value.Should().NotBeNull();
            value.Should().BeOfType<Diagnosis>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<DiagnosisResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task DiagnosesControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            var diagnosis = DataGenerator.GetTestDiagnoses().First();

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(diagnosis);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task DiagnosesControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            var diagnosis = DataGenerator.GetTestDiagnoses().First();

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(diagnosis);

            //Assert
            diagnosisRepo.Verify(r => r.UpdateAsync(It.IsAny<Diagnosis>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Diagnosis;
            value.Should().NotBeNull();
            value.Should().BeOfType<Diagnosis>();
            value?.DiagnosisId.Should().Be(diagnosis.DiagnosisId);
        }

        [Fact]
        public async Task DiagnosesControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            var diagnosis = DataGenerator.GetTestDiagnoses().First();

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>(),
                It.IsAny<Func<IQueryable<Diagnosis>, IIncludableQueryable<Diagnosis, object>>>())).ReturnsAsync(diagnosis).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(diagnosis.DiagnosisId);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task DiagnosesControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<DiagnosesController>>();

            var diagnosis = DataGenerator.GetTestDiagnoses().First();

            var diagnosisRepo = new Mock<IRepository<Diagnosis>>();
            diagnosisRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Diagnosis, bool>>>(),
                It.IsAny<Func<IQueryable<Diagnosis>, IIncludableQueryable<Diagnosis, object>>>())).ReturnsAsync(diagnosis).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Diagnoses).Returns(diagnosisRepo.Object);

            var controller = new DiagnosesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(diagnosis.DiagnosisId);

            //Assert
            diagnosisRepo.Verify(r => r.DeleteAsync(It.IsAny<Diagnosis>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Diagnosis;
            value.Should().NotBeNull();
            value.Should().BeOfType<Diagnosis>();
            value.Should().BeEquivalentTo(diagnosis);
        }
    }
}
