using Microsoft.AspNetCore.Mvc;
using EmployeeManager.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }
    }
}
