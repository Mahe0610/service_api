using Microsoft.EntityFrameworkCore;
using ServiceApi.Models;

namespace ServiceApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EmployeeRecord> EmployeeRecords => Set<EmployeeRecord>();
    public DbSet<CertificateScanLog> CertificateScanLogs => Set<CertificateScanLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeRecord>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<EmployeeRecord>()
            .HasIndex(x => x.Username)
            .IsUnique();

        modelBuilder.Entity<EmployeeRecord>()
            .Property(x => x.UserType)
            .HasMaxLength(20);

        modelBuilder.Entity<EmployeeRecord>()
            .Property(x => x.Username)
            .HasMaxLength(60);

        modelBuilder.Entity<EmployeeRecord>()
            .Property(x => x.Password)
            .HasMaxLength(150);

        modelBuilder.Entity<EmployeeRecord>()
            .Property(x => x.EmployeeName)
            .HasMaxLength(150);

        modelBuilder.Entity<EmployeeRecord>()
            .Property(x => x.Email)
            .HasMaxLength(200);

        modelBuilder.Entity<EmployeeRecord>()
            .Property(x => x.ScannerId)
            .HasMaxLength(100);

        modelBuilder.Entity<EmployeeRecord>()
            .Property(x => x.CertificateCode)
            .HasMaxLength(40);

        modelBuilder.Entity<CertificateScanLog>()
            .HasOne(x => x.EmployeeRecord)
            .WithMany()
            .HasForeignKey(x => x.EmployeeRecordId);
    }
}
