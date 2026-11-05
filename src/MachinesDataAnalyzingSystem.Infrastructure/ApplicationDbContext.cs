using MachinesDataAnalyzingSystem.Core.Domain.Entities;
using MachinesDataAnalyzingSystem.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MachinesDataAnalyzingSystem.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet <AggregatedMachineData> AggregatedMachineDatas { get; set; }
    public DbSet <MachineData> MachineDatas { get; set; }
    public DbSet <Machine> Machines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Machine>().HasData
        (
            new Machine()
            {
                Id = Guid.Parse("444EFAA8-0ACC-4406-9AC2-97CA8E2ADFA6"),
                Name = "Plastic converter"
            },
            new Machine()
            {
                Id = Guid.Parse("E0123916-03A2-4B2D-9598-BE733730FB4E"),
                Name = "Laser cutter"
            },
            new Machine()
            {
                Id = Guid.Parse("7556E854-A525-4889-92FD-7412D9D42949"),
                Name = "Parts assembler"
            },
            new Machine()
            {
                Id = Guid.Parse("B656B81F-0D00-47C2-A836-0B0113223CF3"),
                Name = "Parts cooler"
            },
            new Machine()
            {
                Id = Guid.Parse("CEC67FD6-9B5A-4B8A-80B3-56DACB845C8C"),
                Name = "Heat regulator"
            }
        );
    }
}