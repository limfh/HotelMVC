using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelMVC.Models
{
    public class HotelContextFactory : IDesignTimeDbContextFactory<HotelContext>
    {
        public HotelContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<HotelContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new HotelContext(builder.Options);
        }
    }
}