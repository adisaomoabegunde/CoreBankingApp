using CoreBanking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<PendingRegistration> PendingRegistrations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            builder.Entity<Account>()
                .HasIndex(a => a.AccountNumber)
                .IsUnique();
            builder.Entity<PendingRegistration>()
                .Property(x => x.Role)
                .HasConversion<int>();

            base.OnModelCreating(builder);

            builder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.TransactionReference)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(t => t.TransactionReference)
                    .IsUnique();

                entity.Property(t => t.IdempotencyKey)
                    .IsRequired();

                entity.HasIndex(t => t.IdempotencyKey)
                    .IsUnique();

                entity.Property(t => t.Amount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(t => t.BalanceAfter)
                    .HasColumnType("decimal(18,2)");

                entity.Property(t => t.TransactionType)
                    .HasConversion<int>();

                entity.Property(t => t.Status)
                    .HasConversion<int>();
            });

            builder.Entity<LedgerEntry>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.Amount)
                    .HasColumnType("decimal(18,2");

                entity.Property(l => l.BalanceBefore)
                    .HasColumnType("decimal(18,2)");

                entity.Property(l => l.BalanceAfter)
                    .HasColumnType("decimal(18,2)");

                entity.Property(l => l.EntryType)
                    .HasConversion<int>();
            });

            builder.Entity<LedgerEntry>()
                .HasOne<Transaction>()
                .WithMany()
                .HasForeignKey(l => l.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Account>()
                .Property(a => a.RowVersion)
                .IsRowVersion();
            builder.Entity<Transaction>()
                .HasOne(t => t.SourceAccount)
                .WithMany()
                .HasForeignKey(t => t.SourceAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(t => t.DestinationAccount)
                .WithMany()
                .HasForeignKey(t => t.DestinationAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Transaction>()
                .HasOne<Transaction>()
                .WithMany()
                .HasForeignKey(t => t.ReversedTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Transaction>()
                .HasOne(t => t.ReversedTransaction)
                .WithMany()
                .HasForeignKey(t => t.ReversedTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<LedgerEntry>()
                .HasOne(l => l.Transaction)
                .WithMany(t => t.LedgerEntries)
                .HasForeignKey(l => l.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

        }


    }
}
