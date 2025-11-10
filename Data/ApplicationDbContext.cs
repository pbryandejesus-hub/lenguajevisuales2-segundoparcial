using Microsoft.EntityFrameworkCore;
using lenguajevisuales2_segundoparcial.Models;

namespace lenguajevisuales2_segundoparcial.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ArchivoCliente> ArchivoClientes { get; set; }
        public DbSet<LogApi> LogApis { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().HasKey(c => c.CI);
            modelBuilder.Entity<Cliente>().Property(c => c.FotoCasa1).HasColumnType("varbinary(max)");
            modelBuilder.Entity<Cliente>().Property(c => c.FotoCasa2).HasColumnType("varbinary(max)");
            modelBuilder.Entity<Cliente>().Property(c => c.FotoCasa3).HasColumnType("varbinary(max)");

            modelBuilder.Entity<ArchivoCliente>()
                .HasOne(a => a.Cliente)
                .WithMany(c => c.Archivos)
                .HasForeignKey(a => a.CICliente)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}