using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Implementations;
using HospitalWeb.WebApi.Models.SortStates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HospitalWeb.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly ILogger<AdminsController> _logger;
        private readonly ApiUnitOfWork _uow;
        private readonly AppDbContext _db;

        public AdminsController(
            ILogger<AdminsController> logger,
            ApiUnitOfWork uow,
            AppDbContext db)
        {
            _logger = logger;
            _uow = uow;
            _db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<Admin>> Get(
            string searchString = null,
            AdminSortState sortOrder = AdminSortState.Id,
            int pageSize = 10,
            int pageNumber = 1)
        {
            int totalCount = 0;

            Func<Admin, bool> filter = (a) =>
            {
                bool result = true;

                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    result = a.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase);
                }

                return result;
            };

            Func<IQueryable<Admin>, IOrderedQueryable<Admin>> orderBy = (admins) =>
            {
                totalCount = admins.Count();

                switch (sortOrder)
                {
                    case AdminSortState.NameAsc:
                        admins = admins.OrderBy(a => a.Name);
                        break;
                    case AdminSortState.NameDesc:
                        admins = admins.OrderByDescending(a => a.Name);
                        break;
                    case AdminSortState.SurnameAsc:
                        admins = admins.OrderBy(a => a.Surname);
                        break;
                    case AdminSortState.SurnameDesc:
                        admins = admins.OrderByDescending(a => a.Surname);
                        break;
                    case AdminSortState.EmailAsc:
                        admins = admins.OrderBy(a => a.Email);
                        break;
                    case AdminSortState.EmailDesc:
                        admins = admins.OrderByDescending(a => a.Email);
                        break;
                    case AdminSortState.PhoneAsc:
                        admins = admins.OrderBy(a => a.PhoneNumber);
                        break;
                    case AdminSortState.PhoneDesc:
                        admins = admins.OrderByDescending(a => a.PhoneNumber);
                        break;
                    case AdminSortState.LevelAsc:
                        admins = admins.OrderBy(a => a.IsSuperAdmin);
                        break;
                    case AdminSortState.LevelDesc:
                        admins = admins.OrderByDescending(a => a.IsSuperAdmin);
                        break;
                    default:
                        admins = admins.OrderBy(a => a.Id);
                        break;
                }

                return (IOrderedQueryable<Admin>)admins;
            };

            var admins = await _uow.Admins
                .GetAllAsync(filter: filter, orderBy: orderBy, first: pageSize, offset: (pageNumber - 1) * pageSize);

            var metadata = new
            {
                TotalCount = totalCount,
                Count = admins.Count(),
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(metadata));

            return admins;
        }

        [HttpGet("{searchString}")]
        public async Task<ActionResult<Admin>> Get(string searchString)
        {
            var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Id == searchString || a.Email == searchString);

            if (admin == null)
            {
                return NotFound();
            }

            return new ObjectResult(admin);
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

        [HttpDelete("{Admin}")]
        public async Task<ActionResult<Admin>> Delete(Admin admin)
        {
            if (admin == null)
            {
                return NotFound();
            }

            await _uow.Admins.DeleteAsync(admin);

            return Ok(admin);
        }
    }
}
