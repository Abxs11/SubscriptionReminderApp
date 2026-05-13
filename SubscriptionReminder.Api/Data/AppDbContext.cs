using Microsoft.EntityFrameworkCore;
using SubscriptionReminder.Api.Models;

namespace SubscriptionReminder.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<DebtInquiry> DebtInquiries { get; set; }
    public DbSet<ReminderLog> ReminderLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Tckn).HasMaxLength(11).IsFixedLength().IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            
            entity.HasIndex(e => e.Tckn).IsUnique();
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CustomerId).IsUnique();

            entity.HasOne(d => d.Customer)
                .WithOne(p => p.User)
                .HasForeignKey<User>(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(30).IsRequired();
            entity.Property(e => e.ProviderName).HasMaxLength(150).IsRequired();
            entity.Property(e => e.SubscriberNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();

            entity.HasIndex(e => new { e.CustomerId, e.Type, e.ProviderName, e.SubscriberNumber }).IsUnique();

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(e => e.Period).HasMaxLength(7).IsFixedLength().IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ExternalTransactionId).HasMaxLength(100);
            entity.Property(e => e.FailureReason).HasMaxLength(500);

            entity.HasIndex(e => new { e.SubscriptionId, e.Period })
                  .IsUnique()
                  .HasFilter("\"Status\" = 'Success'");

            entity.HasOne(d => d.Subscription)
                .WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DebtInquiry
        modelBuilder.Entity<DebtInquiry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(e => e.Period).HasMaxLength(7).IsFixedLength().IsRequired();
            entity.Property(e => e.ExternalReference).HasMaxLength(100).IsRequired();

            entity.HasIndex(e => new { e.SubscriptionId, e.Period });

            entity.HasOne(d => d.Subscription)
                .WithMany(p => p.DebtInquiries)
                .HasForeignKey(d => d.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
