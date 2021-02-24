using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Models
{
    public class CapaDbContext : DbContext
    {
        public CapaDbContext(DbContextOptions<CapaDbContext> options) : base(options)
        {
        }
        public DbSet<Department> department { get; set; }

        public DbSet<AdminDepartment> admin_department { get; set; }

        public DbSet<Category> category { get; set; }

        public DbSet<Employee> employee { get; set; }
        public DbSet<LogLogin> log_login { get; set; }
        public DbSet<Admin> admin { get; set; }
        public DbSet<Rule> rule { get; set; }
        public DbSet<Capa> capa { get; set; }
        public DbSet<RelatedWorkUnit> related_work_unit { get; set; }

        public DbSet<RootCause> root_cause { get; set; }

        public DbSet<CorrectionAction> correction_action { get; set; }

        public DbSet<Verification> verification { get; set; }
        public DbSet<EmailLog> email_log { get; set; }
    }
}
