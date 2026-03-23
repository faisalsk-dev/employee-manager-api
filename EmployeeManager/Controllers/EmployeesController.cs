using Microsoft.AspNetCore.Mvc;
using EmployeeManager.Api.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeManager.Api.Models;
using EmployeeManager.Api.DTOs;
using EmployeeManager.Services;
using System.Runtime.CompilerServices;

namespace EmployeeManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmployeeService _employeeService;

        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(int page = 1, int pageSize = 5, string? sortBy = null, string? sortOrder = "asc")
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be greater than 0.");
            }
            var results = await _employeeService.GetEmployees(page, pageSize, sortBy, sortOrder);

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Department = dto.Department,
                Phone = dto.Phone,
                Address = dto.Address,
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployees), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = dto.Name;
            employee.Email = dto.Email;
            employee.Department = dto.Department;
            employee.Phone = dto.Phone;
            employee.Address = dto.Address;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            //_context.Employees.Remove(employee);
            employee.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees(string? name, string? department)
        {
            var query = _context.Employees
                .Where(e => !e.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(e => e.Department.Contains(department));
            }

            var employees = await query
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = e.Department
                })
                .ToListAsync();

            return Ok(employees);
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreEmployee(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            if (!employee.IsDeleted)
            {
                return BadRequest("Employee is not deleted.");
            }

            employee.IsDeleted = false;

            await _context.SaveChangesAsync();

            return Ok("Employee restored successfully.");
        }

    }
}
