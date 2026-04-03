using EmployeeManager.Api.DTOs;
using EmployeeManager.Models;

namespace EmployeeManager.Services
{
    public interface IEmployeeService
    {
        Task<PagedResult<EmployeeResponseDto>> GetEmployees(
            int page,
            int pageSize,
            string? name,
            string? department,
            string? sortBy,
            string? sortOrder);

        Task<EmployeeResponseDto> GetEmployeeById(int id);

        Task<EmployeeResponseDto> CreateEmployee(CreateEmployeeDto dto);

        Task<EmployeeResponseDto> UpdateEmployee(int id, UpdateEmployeeDto dto);

        Task DeleteEmployee(int id);

        Task RestoreEmployee(int id);
    }
}
