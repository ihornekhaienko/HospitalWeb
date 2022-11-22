﻿using AutoMapper;
using FluentAssertions;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Interfaces;
using HospitalWeb.Tests.Services;
using HospitalWeb.WebApi.Controllers;
using HospitalWeb.WebApi.Models.ResourceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace HospitalWeb.WebApi.Tests
{
    public class AppointmentsControllerTests
    {
        [Fact]
        public async Task AppointmentsControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var appointments = DataGenerator.GetTestAppointments();

            var headerMock = new Mock<IHeaderDictionary>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Response.Headers).Returns(headerMock.Object);

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            appointmentRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Appointment, bool>>(),
                It.IsAny<Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>>>(),
                It.IsAny<Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(appointments);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            //Act
            var result = await controller.Get();

            //Assert
            appointmentRepo.Verify();
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<Appointment>>();
            result.Should().AllBeOfType<Appointment>();
            result.Count().Should().Be(appointments.Count);
        }

        [Fact]
        public async Task AppointmentsControllerFilter_ShouldReturnFilteredValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var appointments = DataGenerator.GetTestAppointments();
            var state = State.Missed;
            DateTime toDate = appointments.GroupBy(a => a.AppointmentDate)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .First();

            var headerMock = new Mock<IHeaderDictionary>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Response.Headers).Returns(headerMock.Object);

            var correct = appointments.Where(a =>
                a.State == state &&
                DateTime.Compare(toDate, a.AppointmentDate) >= 0).ToList();

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            appointmentRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Appointment, bool>>(),
                It.IsAny<Func<IQueryable<Appointment>, IOrderedQueryable<Appointment>>>(),
                It.IsAny<Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(correct);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            //Act
            var result = await controller.Get(state: (int)state, toDate: toDate);

            //Assert
            appointmentRepo.Verify();
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<Appointment>>();
            result.Should().AllBeOfType<Appointment>();
            result.Count().Should().Be(appointments.Count);
        }

        [Fact]
        public async Task AppointmentsControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            int id = 5;
            var appointments = DataGenerator.GetTestAppointments();
            var correct = appointments.Where(a => a.AppointmentId == id).First();

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            appointmentRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Appointment, bool>>>(),
                It.IsAny<Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(5);

            //Assert
            appointmentRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Appointment;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task AppointmentsControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            int id = 1000;
            var appointments = DataGenerator.GetTestAppointments();

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            appointmentRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Appointment, bool>>>(),
                It.IsAny<Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task AppointmentsControllerGetByDoctorDate_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var diagnoses = DataGenerator.GetTestDiagnoses();
            var doctors = DataGenerator.GetTestDoctors();
            var patients = DataGenerator.GetTestPatients();
            var appointments = DataGenerator.GetTestAppointments(diagnoses, doctors, patients);
            var correct = appointments.First();

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            appointmentRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Appointment, bool>>>(),
                It.IsAny<Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(correct.DoctorId, correct.AppointmentDate);

            //Assert
            appointmentRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Appointment;
            value.Should().NotBeNull();
            value?.DoctorId.Should().Be(correct.DoctorId);
            value?.AppointmentDate.Should().Be(correct.AppointmentDate);
            value.Should().Be(correct);
        }

        [Fact]
        public async Task AppointmentsControllerGetByDoctorDay_ShouldNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var diagnoses = DataGenerator.GetTestDiagnoses();
            var doctors = DataGenerator.GetTestDoctors();
            var patients = DataGenerator.GetTestPatients();
            var appointments = DataGenerator.GetTestAppointments(diagnoses, doctors, patients);

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            appointmentRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Appointment, bool>>>(),
                It.IsAny<Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(Guid.NewGuid().ToString(), DateTime.Today);

            //Assert
            appointmentRepo.Verify();
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task AppointmentsControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var entity = DataGenerator.GetTestAppointments().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Appointment, AppointmentResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Appointment, AppointmentResourceModel>(entity);

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AppointmentsControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var entity = DataGenerator.GetTestAppointments().First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Appointment, AppointmentResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Appointment, AppointmentResourceModel>(entity);

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            appointmentRepo.Verify(r => r.CreateAsync(It.IsAny<Appointment>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Appointment;
            value.Should().NotBeNull();
            value.Should().BeOfType<Appointment>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<AppointmentResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task AppointmentsControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var appointment = DataGenerator.GetTestAppointments().First();

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(appointment);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AppointmentsControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var appointment = DataGenerator.GetTestAppointments().First();

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(appointment);

            //Assert
            appointmentRepo.Verify(r => r.UpdateAsync(It.IsAny<Appointment>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Appointment;
            value.Should().NotBeNull();
            value.Should().BeOfType<Appointment>();
            value?.AppointmentId.Should().Be(appointment.AppointmentId);
        }

        [Fact]
        public async Task AppointmentsControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var appointment = DataGenerator.GetTestAppointments().First();

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            appointmentRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Appointment, bool>>>(),
                It.IsAny<Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>>>())).ReturnsAsync(appointment).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(appointment.AppointmentId);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AppointmentsControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AppointmentsController>>();

            var appointment = DataGenerator.GetTestAppointments().First();

            var appointmentRepo = new Mock<IRepository<Appointment>>();
            appointmentRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Appointment, bool>>>(),
                It.IsAny<Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>>>())).ReturnsAsync(appointment).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Appointments).Returns(appointmentRepo.Object);

            var controller = new AppointmentsController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(appointment.AppointmentId);

            //Assert
            appointmentRepo.Verify(r => r.DeleteAsync(It.IsAny<Appointment>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Appointment;
            value.Should().NotBeNull();
            value.Should().BeOfType<Appointment>();
            value.Should().BeEquivalentTo(appointment);
        }
    }
}
