using Microsoft.EntityFrameworkCore;

namespace TieChef.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }



        public DbSet<Models.Entities.Staff> Staffs { get; set; }
        public DbSet<Models.Entities.Receipt> Receipts { get; set; }
        public DbSet<Models.Entities.DiningTable> DiningTables { get; set; }
        public DbSet<Models.Entities.Dish> Dishes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
