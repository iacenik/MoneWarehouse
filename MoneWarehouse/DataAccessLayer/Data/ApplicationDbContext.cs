using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
   public class ApplicationDbContext : DbContext

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Formula> Formulas { get; set; }
        public DbSet<InjectionDaily> InjectionDailies { get; set; }
        public DbSet<Injection> Injections { get; set; }
        public DbSet<InjectionStock> InjectionStocks { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Plıntus> Plıntuses { get; set; }
        public DbSet<PlıntusDaily> PlıntusDailies { get; set; }
        public DbSet<PlıntusStock> PlıntusStocks { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Sales> Saleses { get; set; }
        public DbSet<SalesDetail> SalesDetails { get; set; }    
        public DbSet<Size> Sizes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client relationships
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Sales)
                .WithOne(s => s.Client)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Codes relationships
            modelBuilder.Entity<Codes>()
                .HasOne(c => c.Size)
                .WithMany(s => s.Codes)
                .HasForeignKey(c => c.SizeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Injection relationships
            modelBuilder.Entity<Injection>()
                .HasOne(i => i.Size)
                .WithMany()
                .HasForeignKey(i => i.SizeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Injection>()
                .HasOne(i => i.Codes)
                .WithMany()
                .HasForeignKey(i => i.CodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // InjectionDaily relationships
            modelBuilder.Entity<InjectionDaily>()
                .HasOne(id => id.InjectionStock)
                .WithMany(i => i.InjectionDailies)
                .HasForeignKey(id => id.InjectionStockId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InjectionDaily>()
                .HasOne(id => id.Employee)
                .WithMany()
                .HasForeignKey(id => id.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Plıntus relationships
            modelBuilder.Entity<Plıntus>()
                .HasOne(p => p.Size)
                .WithMany()
                .HasForeignKey(p => p.SizeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Plıntus>()
                .HasOne(p => p.Codes)
                .WithMany()
                .HasForeignKey(p => p.CodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // PlıntusDaily relationships
            modelBuilder.Entity<PlıntusDaily>()
                .HasOne(pd => pd.PlıntusStock)
                .WithMany(p => p.PlıntusDailies)
                .HasForeignKey(pd => pd.PlıntusStockId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlıntusDaily>()
                .HasOne(pd => pd.Employee)
                .WithMany()
                .HasForeignKey(pd => pd.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Request relationships
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Employee)
                .WithMany()
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sales relationships
            modelBuilder.Entity<Sales>()
                .HasOne(s => s.Employee)
                .WithMany()
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // SalesDetail relationships
            modelBuilder.Entity<SalesDetail>()
                .HasOne(sd => sd.Sales)
                .WithMany(s => s.SalesDetails)
                .HasForeignKey(sd => sd.SalesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesDetail>()
                .HasOne(sd => sd.PlıntusStock)
                .WithMany()
                .HasForeignKey(sd => sd.PlıntusStockId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<SalesDetail>()
                .HasOne(sd => sd.InjectionStock)
                .WithMany()
                .HasForeignKey(sd => sd.InjectionStockId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
