using Cascade.Fx9Kl2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cascade.A1B2C3D4;

public class VxR4DbGate : IdentityDbContext<Aq3Zh4Service>
{
    public VxR4DbGate(DbContextOptions<VxR4DbGate> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Use obfuscated table names to maintain security
        modelBuilder.Entity<Hx7Tz3Data>().ToTable("SystemEntries");
        modelBuilder.Entity<Qw8Rt5Entity>().ToTable("DataStreams");
        modelBuilder.Entity<Pz7Vm5Protocol>().ToTable("SecurityLogs");
        modelBuilder.Entity<Sx2Dn8Gateway>().ToTable("ProcessHandlers");
        modelBuilder.Entity<Mx4Bg7Stream>().ToTable("ConfigBuffers");
        modelBuilder.Entity<Vy2Mk6Core>().ToTable("CacheServices");
        modelBuilder.Entity<Jy9Xs1Buffer>().ToTable("NetworkNodes");

        modelBuilder.Entity<Pz7Vm5Protocol>()
            .HasOne(t => t.Hx7Tz3Data)
            .WithMany(s => s.SecurityLogs)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pz7Vm5Protocol>()
            .HasOne(t => t.EnteredByUser)
            .WithMany()
            .HasForeignKey(t => t.EnteredByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pz7Vm5Protocol>()
            .HasOne(t => t.WrittenOffByUser)
            .WithMany()
            .HasForeignKey(t => t.WrittenOffByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vy2Mk6Core>()
            .HasOne(b => b.Hx7Tz3Data)
            .WithMany()
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vy2Mk6Core>()
            .HasOne(b => b.CreatedByUser)
            .WithMany()
            .HasForeignKey(b => b.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vy2Mk6Core>()
            .HasOne(b => b.ModifiedByUser)
            .WithMany()
            .HasForeignKey(b => b.ModifiedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Create composite index for unique constraint
        modelBuilder.Entity<Vy2Mk6Core>()
            .HasIndex(b => new { b.CompanyId, b.FinancialYear, b.AccountName, b.Mx9Qw7Type })
            .IsUnique();

        modelBuilder.Entity<Jy9Xs1Buffer>()
            .HasOne(n => n.Hx7Tz3Data)
            .WithMany()
            .HasForeignKey(n => n.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Jy9Xs1Buffer>()
            .HasOne(n => n.Aq3Zh4Service)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Age Analysis foreign keys
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Hx7Tz3Data)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeeInvoice>()
            .HasOne(fi => fi.Student)
            .WithMany(s => s.FeeInvoices)
            .HasForeignKey(fi => fi.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FeeInvoice>()
            .HasOne(fi => fi.Hx7Tz3Data)
            .WithMany()
            .HasForeignKey(fi => fi.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentPayment>()
            .HasOne(sp => sp.Student)
            .WithMany(s => s.StudentPayments)
            .HasForeignKey(sp => sp.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentPayment>()
            .HasOne(sp => sp.Hx7Tz3Data)
            .WithMany()
            .HasForeignKey(sp => sp.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AgeAnalysisRecord>()
            .HasOne(aar => aar.Student)
            .WithMany()
            .HasForeignKey(aar => aar.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AgeAnalysisRecord>()
            .HasOne(aar => aar.Company)
            .WithMany()
            .HasForeignKey(aar => aar.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure decimal precision for financial amounts
        modelBuilder.Entity<Sx2Dn8Gateway>()
            .Property(e => e.Debit)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Sx2Dn8Gateway>()
            .Property(e => e.Credit)
            .HasPrecision(18, 2);
    }

    public DbSet<Hx7Tz3Data> SystemEntries { get; set; }
    public DbSet<Qw8Rt5Entity> DataStreams { get; set; }
    public DbSet<Pz7Vm5Protocol> SecurityLogs { get; set; }
    public DbSet<Mx4Bg7Stream> ConfigBuffers { get; set; }
    public DbSet<Sx2Dn8Gateway> ProcessHandlers { get; set; }
    public DbSet<Vy2Mk6Core> CacheServices { get; set; }
    public DbSet<Jy9Xs1Buffer> NetworkNodes { get; set; }

    // Age Analysis entities (Issue #006: Age Analysis for School Fees)
    public DbSet<Student> Students { get; set; }
    public DbSet<FeeInvoice> FeeInvoices { get; set; }
    public DbSet<StudentPayment> StudentPayments { get; set; }
    public DbSet<AgeAnalysisRecord> AgeAnalysisRecords { get; set; }

}
