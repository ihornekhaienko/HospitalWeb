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
    public class SchedulesControllerTests
    {
        [Fact]
        public async Task SchedulesControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var schedules = DataGenerator.GetTestSchedules();

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            scheduleRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Schedule, bool>>(),
                It.IsAny<Func<IQueryable<Schedule>, IOrderedQueryable<Schedule>>>(),
                It.IsAny<Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(schedules);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var result = await controller.Get();

            //Assert
            scheduleRepo.Verify();
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<Schedule>>();
            result.Should().AllBeOfType<Schedule>();
            result.Count().Should().Be(schedules.Count);
        }

        [Fact]
        public async Task SchedulesControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            int id = 5;
            var schedules = DataGenerator.GetTestSchedules();
            var correct = schedules.Where(a => a.ScheduleId == id).First();

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            scheduleRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schedule, bool>>>(),
                It.IsAny<Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(5);

            //Assert
            scheduleRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Schedule;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task SchedulesControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            int id = 1000;
            var schedules = DataGenerator.GetTestSchedules();

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            scheduleRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schedule, bool>>>(),
                It.IsAny<Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task SchedulesControllerGetByDoctorDay_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var doctors = DataGenerator.GetTestDoctors();
            var schedules = DataGenerator.GetTestSchedules(doctors);
            var correct = schedules.First();

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            scheduleRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schedule, bool>>>(),
                It.IsAny<Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(correct.DoctorId, correct.DayOfWeek.ToString());

            //Assert
            scheduleRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Schedule;
            value.Should().NotBeNull();
            value?.DoctorId.Should().Be(correct.DoctorId);
            value?.DayOfWeek.Should().Be(correct.DayOfWeek);
            value.Should().Be(correct);
        }

        [Fact]
        public async Task SchedulesControllerGetByDoctorDay_ShouldNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var doctors = DataGenerator.GetTestDoctors();
            var schedules = DataGenerator.GetTestSchedules(doctors);

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            scheduleRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schedule, bool>>>(),
                It.IsAny<Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(Guid.NewGuid().ToString(), DayOfWeek.Monday.ToString());

            //Assert
            scheduleRepo.Verify();
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task SchedulesControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var entity = DataGenerator.GetTestSchedules(1).First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Schedule, ScheduleResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Schedule, ScheduleResourceModel>(entity);

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task SchedulesControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var entity = DataGenerator.GetTestSchedules(1).First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Schedule, ScheduleResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Schedule, ScheduleResourceModel>(entity);

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            scheduleRepo.Verify(r => r.CreateAsync(It.IsAny<Schedule>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Schedule;
            value.Should().NotBeNull();
            value.Should().BeOfType<Schedule>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<ScheduleResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task SchedulesControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var schedule = DataGenerator.GetTestSchedules(1).First();

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(schedule);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task SchedulesControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var schedule = DataGenerator.GetTestSchedules(1).First();

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(schedule);

            //Assert
            scheduleRepo.Verify(r => r.UpdateAsync(It.IsAny<Schedule>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Schedule;
            value.Should().NotBeNull();
            value.Should().BeOfType<Schedule>();
            value?.ScheduleId.Should().Be(schedule.ScheduleId);
        }

        [Fact]
        public async Task SchedulesControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var schedule = DataGenerator.GetTestSchedules(1).First();

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            scheduleRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schedule, bool>>>(),
                It.IsAny<Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>>>())).ReturnsAsync(schedule).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(schedule.ScheduleId);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task SchedulesControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<SchedulesController>>();

            var schedule = DataGenerator.GetTestSchedules(1).First();

            var scheduleRepo = new Mock<IRepository<Schedule>>();
            scheduleRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Schedule, bool>>>(),
                It.IsAny<Func<IQueryable<Schedule>, IIncludableQueryable<Schedule, object>>>())).ReturnsAsync(schedule).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Schedules).Returns(scheduleRepo.Object);

            var controller = new SchedulesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(schedule.ScheduleId);

            //Assert
            scheduleRepo.Verify(r => r.DeleteAsync(It.IsAny<Schedule>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Schedule;
            value.Should().NotBeNull();
            value.Should().BeOfType<Schedule>();
            value.Should().BeEquivalentTo(schedule);
        }
    }
}