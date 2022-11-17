using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly ILogger<AdminsController> _logger;
        private readonly UnitOfWork _uow;

        public AdminsController(
            ILogger<AdminsController> logger,
            UnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        [HttpGet]
        public async Task<IEnumerable<Admin>> Get()
        {
            return await _uow.Admins.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> Get(string id)
        {
            return await _uow.Admins.GetAsync(a => a.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult<Admin>> Post(Admin admin)
        {
            if (admin == null)
            {
                return BadRequest();
            }

            await _uow.Admins.CreateAsync(admin);

            return Ok(admin);
        }

        [HttpPut]
        public async Task<ActionResult<Admin>> Put(Admin admin)
        {
            if (admin == null)
            {
                return BadRequest();
            }

            await _uow.Admins.UpdateAsync(admin);

            return Ok(admin);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Admin>> Delete(string id)
        {
            var admin = await _uow.Admins.GetAsync(a => a.Id == id);

            if (admin == null)
            {
                return NotFound();
            }

            await _uow.Admins.DeleteAsync(admin);

            return Ok(admin);
        }
    }
}
