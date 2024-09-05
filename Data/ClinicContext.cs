using ClinicManagementAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace ClinicManagementAPI.Data
{
    public class ClinicContext : DbContext
    {
        public DbSet<Estate> Uchastki { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Cabinet> Cabinets { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        public ClinicContext(DbContextOptions<ClinicContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
