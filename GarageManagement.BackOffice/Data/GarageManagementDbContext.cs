using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GarageManagement.BackOffice.Models;

namespace GarageManagement.BackOffice.Data
{
    public class GarageManagementDbContext : DbContext
    {
        public GarageManagementDbContext(DbContextOptions<GarageManagementDbContext> options) : base(options) { }
        
        public DbSet<GarageManagement.BackOffice.Models.Brand> Brand { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.Model> Model { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.User> User { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.Vehicle> Vehicle { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.Appointment> Appointment { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.Reparation> Reparation { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.Employee> Employee { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.ReparationEmployee> ReparationEmployee { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.ReparationType> ReparationType { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.ReparationDetail> ReparationDetail { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.Facturation> Facturation { get; set; } = default!;
        public DbSet<GarageManagement.BackOffice.Models.FacturationDetail> FacturationDetail { get; set; } = default!;
    }
}
