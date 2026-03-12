using EmployeeManager.Api.Data;
using EmployeeManager.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeResponseDto>> GetEmployees(int page, int pageSize)
        {
            var query = _context.Employees
                .Where(e => !e.IsDeleted);

            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = e.Department
                })
                .ToListAsync();

            return employees;
        }
    }
}
