using EmployeeManager.Api.Data;
using EmployeeManager.Api.DTOs;
using EmployeeManager.Models;
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

        public async Task<PagedResult<EmployeeResponseDto>> GetEmployees(int page, int pageSize, string? sortBy = null, string? sortOrder = "asc")
        {
            var query = _context.Employees
                .Where(e => !e.IsDeleted);

            // Sorting logic
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.ToLower() == "name")
                {
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(e => e.Name)
                        : query.OrderBy(e => e.Name);
                }
                else if (sortBy.ToLower() == "department")
                {
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(e => e.Department)
                        : query.OrderBy(e => e.Department);
                }
            }

            var totalRecords = await query.CountAsync();

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

            var result = new PagedResult<EmployeeResponseDto>
            {
                Data = employees,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };

            return result;
        }
    }
}
