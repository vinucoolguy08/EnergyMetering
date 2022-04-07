using EnergyMetering.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyMetering.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<MeterReading> MeterReading { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    DeliveryEmail = "Sam@hotmail.com",
                    Status = "Approved",
                    MeterId = Guid.NewGuid(),
                },
                new Customer
                {
                    CustomerId = 2,
                    DeliveryEmail = "Patrick@hotmail.com",
                    Status = "Approved",
                    MeterId = Guid.NewGuid(),
                }
            );

            modelBuilder.Entity<MeterReading>().HasData(
                new MeterReading
                {
                    Id = 1,
                    Kilowatt = 12.5,
                    TimeStamp = DateTime.Now.AddDays(-1),
                    CustomerId = 1
                },
                new MeterReading
                {
                    Id = 2,
                    Kilowatt = 22.5,
                    TimeStamp = DateTime.Now.AddDays(2),
                    CustomerId = 1,
                },
                new MeterReading
                {
                    Id = 3,
                    Kilowatt = 22.5,
                    TimeStamp = DateTime.Now.AddDays(-1),
                    CustomerId = 2,
                },
                new MeterReading
                {
                    Id = 4,
                    Kilowatt = 22.5,
                    TimeStamp = DateTime.Now.AddDays(2),
                    CustomerId = 2,
                }
            );
        }
    }
}