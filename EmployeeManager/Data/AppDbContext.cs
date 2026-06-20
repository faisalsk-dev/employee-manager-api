using Microsoft.EntityFrameworkCore;
using EmployeeManager.Api.Models;

namespace EmployeeManager.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();

            /*below code add these data automatically while creation of <Employee> table in database*/
            //modelBuilder.Entity<Employee>().HasData(
            //    new Employee { Id = 1, Name = "John Doe", Position = "Software Engineer", Department = "IT" },
            //    new Employee { Id = 2, Name = "Jane Smith", Position = "Project Manager", Department = "IT" },
            //    new Employee { Id = 3, Name = "Alice Johnson", Position = "HR Specialist", Department = "HR" }
            //);    
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
