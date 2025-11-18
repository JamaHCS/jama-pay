using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Amount).HasPrecision(18, 2);

                entity.OwnsMany(o => o.Fees, fee =>
                {
                    fee.ToJson();
                    fee.Property(f => f.Amount).HasPrecision(18, 2);
                });

                entity.OwnsMany(o => o.Taxes, tax =>
                {
                    tax.ToJson();
                    tax.Property(t => t.Amount).HasPrecision(18, 2);
                });

                entity.OwnsMany(o => o.Products, product =>
                {
                    product.ToJson();
                    product.Property(t => t.UnitPrice).HasPrecision(18, 2);
                });
            });
        }
    }
}
