using Mango.Services.CoupanAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CoupanAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Coupan> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Coupan>().HasData(new Coupan
            {
                CoupanId = 1,
                CoupanCode = "10OFF",
                DiscountAmount = 10,
                MinAmount = 20
            });
            modelBuilder.Entity<Coupan>().HasData(new Coupan
            {
                CoupanId = 2,
                CoupanCode = "20OFF",
                DiscountAmount = 20,
                MinAmount = 40
            });

        }
    }
}
