using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly ILogger<AddressesController> _logger;
        private readonly UnitOfWork _uow;

        public AddressesController(
            ILogger<AddressesController> logger,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        [HttpGet]
        public async Task<IEnumerable<Address>> Get()
        {
            return await _uow.Addresses.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> Get(int id)
        {
            return await _uow.Addresses.GetAsync(a => a.AddressId == id);
        }

        [HttpPost]
        public async Task<ActionResult<Address>> Post(Address address)
        {
            if (address == null)
            {
                return BadRequest();
            }

            await _uow.Addresses.CreateAsync(address);

            return Ok(address);
        }

        [HttpPut]
        public async Task<ActionResult<Address>> Put(Address address)
        {
            if (address == null)
            {
                return BadRequest();
            }

            await _uow.Addresses.UpdateAsync(address);

            return Ok(address);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Address>> Put(int id)
        {
            var address = await _uow.Addresses.GetAsync(a => a.AddressId == id);

            if (address == null)
            {
                return NotFound();
            }

            await _uow.Addresses.DeleteAsync(address);

            return Ok(address);
        }
    }
}
