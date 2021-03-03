using Microsoft.EntityFrameworkCore;
using DNXtensions;

namespace DnxSampleApi
{
    public class FooContext : DbContext
    {
        public FooContext(DbContextOptions<FooContext> options) : base(options) =>
            Database.EnsureCreated();        
        public DbSet<Record<FooType>> Foos { get; set; }

    }
}