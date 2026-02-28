using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Api.DTOs
{
    public class UpdateEmployeeDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
