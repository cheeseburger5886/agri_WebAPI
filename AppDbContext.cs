using Microsoft.EntityFrameworkCore;
using agriWebAPI.Model.Department;
using agriWebAPI.Model.Employee;

namespace agriWebAPI
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<department> Departments { get; set; }
        public DbSet<employee> Employees { get; set; }
    }
}
