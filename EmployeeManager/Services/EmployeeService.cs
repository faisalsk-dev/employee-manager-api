using EmployeeManager.Api.Data;
using EmployeeManager.Api.DTOs;
using EmployeeManager.Api.Models;
using EmployeeManager.Exceptions;
using EmployeeManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EmployeeManager.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<EmployeeResponseDto>> GetEmployees(int page, int pageSize, string? name, string? department, string? sortBy = null, string? sortOrder = "asc")
        {
            var query = _context.Employees
                .Where(e => !e.IsDeleted);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(e => e.Department.Contains(department));
            }

            var allowedSortFields = new[] { "name", "department" };

            if (!string.IsNullOrEmpty(sortBy) && !allowedSortFields.Contains(sortBy.ToLower()))
            {
                throw new BadRequestException("Invalid sort field");
            }

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
        public async Task<EmployeeResponseDto> GetEmployeeById(int id)
        {
            var employee = await _context.Employees
                .Where(e => !e.IsDeleted && e.Id == id)
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = e.Department
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }

            return employee;
        }
        public async Task<EmployeeResponseDto> CreateEmployee(CreateEmployeeDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Department = dto.Department,
                Phone = dto.Phone,
                Address = dto.Address,
                IsDeleted = false
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Phone = employee.Phone,
                Address = employee.Address

            };
        }
        public async Task<EmployeeResponseDto> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }

            employee.Name = dto.Name;
            employee.Email = dto.Email;
            employee.Department = dto.Department;
            employee.Phone = dto.Phone;
            employee.Address = dto.Address;

            await _context.SaveChangesAsync();

            return new EmployeeResponseDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                Phone = employee.Phone,
                Address = employee.Address
            };
        }
        public async Task DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }
            if (employee.IsDeleted)
            {
                throw new BadRequestException("Employee already deleted");
            }

            employee.IsDeleted = true;

            await _context.SaveChangesAsync();
        }
        public async Task RestoreEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }

            if (!employee.IsDeleted)
            {
                throw new BadRequestException("Employee is not deleted");
            }

            employee.IsDeleted = false;

            await _context.SaveChangesAsync();
        }
    }
}
