using AutoMapper;
using FluentAssertions;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Services.Interfaces;
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
    public class AddressesControllerTests
    {
        [Fact]
        public async Task AddressesControllerGetAll_ShouldReturnValues()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            var addresses = DataGenerator.GetTestAddresses();

            var addressRepo = new Mock<IRepository<Address>>();
            addressRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Func<Address, bool>>(),
                It.IsAny<Func<IQueryable<Address>, IOrderedQueryable<Address>>>(),
                It.IsAny<Func<IQueryable<Address>, IIncludableQueryable<Address, object>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                )).ReturnsAsync(addresses);
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var response = await controller.Get();
            var result = response.Result as ObjectResult;
            var value = result?.Value as IEnumerable<Address>;

            //Assert
            addressRepo.Verify();
            value.Should().NotBeNullOrEmpty();
            value.Should().BeOfType<List<Address>>();
            value.Should().AllBeOfType<Address>();
            value.Count().Should().Be(addresses.Count);
        }

        [Fact]
        public async Task AddressesControllerGetById_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            int id = 5;
            var addresses = DataGenerator.GetTestAddresses();
            var correct = addresses.Where(a => a.AddressId == id).First();

            var addressRepo = new Mock<IRepository<Address>>();
            addressRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Address, bool>>>(),
                It.IsAny<Func<IQueryable<Address>, IIncludableQueryable<Address, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);       

            //Act
            var actionResult = await controller.Get(5);

            //Assert
            addressRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Address;
            value.Should().NotBeNull();
            value.Should().Be(correct);
        }

        [Fact]
        public async Task AddressesControllerGetById_ShouldReturnNotFound()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            int id = 1000;
            var addresses = DataGenerator.GetTestAddresses();

            var addressRepo = new Mock<IRepository<Address>>();
            addressRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Address, bool>>>(),
                It.IsAny<Func<IQueryable<Address>, IIncludableQueryable<Address, object>>>())).ReturnsAsync(value: null).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(id);

            //Assert
            var result = actionResult.Result as NotFoundResult;
        }

        [Fact]
        public async Task AddressesControllerGetAddressLocality_ShouldReturnCorrect()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            var localities = DataGenerator.GetTestLocalities();
            var addresses = DataGenerator.GetTestAddressesWithId(localities);
            var correct = addresses.First();

            var addressRepo = new Mock<IRepository<Address>>();
            addressRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Address, bool>>>(),
                It.IsAny<Func<IQueryable<Address>, IIncludableQueryable<Address, object>>>())).ReturnsAsync(correct).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Get(correct.FullAddress, correct.Locality.LocalityName);

            //Assert
            addressRepo.Verify();
            var result = actionResult.Result as ObjectResult;
            result.Should().NotBeNull();
            var value = result?.Value as Address;
            value.Should().NotBeNull();
            value?.FullAddress.Should().Be(correct.FullAddress);
            value?.Locality?.LocalityName.Should().Be(correct.Locality.LocalityName);
            value.Should().Be(correct);
        }

        [Fact]
        public async Task AddressesControllerPost_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            var entity = DataGenerator.GetTestAddresses(1).First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Address, AddressResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Address, AddressResourceModel>(entity);

            var addressRepo = new Mock<IRepository<Address>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AddressesControllerPost_ShouldCreate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            var entity = DataGenerator.GetTestAddresses(1).First();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Address, AddressResourceModel>());
            var mapper = new Mapper(config);
            var model = mapper.Map<Address, AddressResourceModel>(entity);

            var addressRepo = new Mock<IRepository<Address>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Post(model);

            //Assert
            addressRepo.Verify(r => r.CreateAsync(It.IsAny<Address>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Address;
            value.Should().NotBeNull();
            value.Should().BeOfType<Address>();
            value.Should().BeEquivalentTo(model, o => o.ComparingByMembers<AddressResourceModel>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task AddressesControllerPut_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            var address = DataGenerator.GetTestAddresses(1).First();

            var addressRepo = new Mock<IRepository<Address>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(address);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AddressesControllerPut_ShouldUpdate()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            var address = DataGenerator.GetTestAddresses(1).First();

            var addressRepo = new Mock<IRepository<Address>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Put(address);

            //Assert
            addressRepo.Verify(r => r.UpdateAsync(It.IsAny<Address>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Address;
            value.Should().NotBeNull();
            value.Should().BeOfType<Address>();
            value?.AddressId.Should().Be(address.AddressId);
        }

        [Fact]
        public async Task AddressesControllerDelete_ShouldReturnOk()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            var address = DataGenerator.GetTestAddresses(1).First();

            var addressRepo = new Mock<IRepository<Address>>();
            addressRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Address, bool>>>(),
                It.IsAny<Func<IQueryable<Address>, IIncludableQueryable<Address, object>>>())).ReturnsAsync(address).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(address.AddressId);

            //Assert
            var result = actionResult.Result as OkResult;
        }

        [Fact]
        public async Task AddressesControllerDelete_ShouldDelete()
        {
            //Arrange
            var logger = Mock.Of<ILogger<AddressesController>>();

            var address = DataGenerator.GetTestAddresses(1).First();

            var addressRepo = new Mock<IRepository<Address>>();
            addressRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Address, bool>>>(),
                It.IsAny<Func<IQueryable<Address>, IIncludableQueryable<Address, object>>>())).ReturnsAsync(address).Verifiable();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Addresses).Returns(addressRepo.Object);

            var controller = new AddressesController(logger, uow.Object);

            //Act
            var actionResult = await controller.Delete(address.AddressId);

            //Assert
            addressRepo.Verify(r => r.DeleteAsync(It.IsAny<Address>()), Times.Once());
            var result = actionResult.Result as OkObjectResult;
            var value = result?.Value as Address;
            value.Should().NotBeNull();
            value.Should().BeOfType<Address>();
            value.Should().BeEquivalentTo(address);
        }
    }
}