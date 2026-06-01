using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database;

public class CourseDbContext(DbContextOptions<CourseDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<CreditApplication> CreditApplications => Set<CreditApplication>();
    public DbSet<PaymentRecipient> PaymentRecipients => Set<PaymentRecipient>();
    public DbSet<PaymentTemplate> PaymentTemplates => Set<PaymentTemplate>();
    public DbSet<DepositProduct> DepositProducts => Set<DepositProduct>();
    public DbSet<UserDeposit> UserDeposits => Set<UserDeposit>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
            entity.Property(e => e.DebtBalance).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(8, 4);
            entity.HasOne(e => e.User).WithMany(u => u.Accounts).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.CreditApplication).WithOne(c => c.IssuedAccount)
                .HasForeignKey<Account>(e => e.CreditApplicationId);
            entity.HasQueryFilter(e => !e.IsDeleted && !e.User.IsDeleted);
        });
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.CreditedAmount).HasPrecision(18, 2);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);
            entity.HasOne(e => e.FromAccount).WithMany().HasForeignKey(e => e.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ToAccount).WithMany().HasForeignKey(e => e.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<CreditApplication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AmountRequested).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(8, 4);
            entity.Property(e => e.MonthlyPayment).HasPrecision(18, 2);
            entity.HasOne(e => e.User).WithMany(u => u.CreditApplications).HasForeignKey(e => e.UserId);
            entity.HasQueryFilter(e => !e.User.IsDeleted);
        });
        modelBuilder.Entity<PaymentRecipient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.BankAccount).HasMaxLength(50);
        });
        modelBuilder.Entity<PaymentTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Nickname).HasMaxLength(100);
            entity.HasOne(e => e.User).WithMany(u => u.PaymentTemplates).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Recipient).WithMany().HasForeignKey(e => e.RecipientId);
            entity.HasQueryFilter(e => !e.User.IsDeleted);
        });
        modelBuilder.Entity<DepositProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.MinAmount).HasPrecision(18, 2);
            entity.Property(e => e.InterestRatePerYear).HasPrecision(8, 4);
            entity.Property(e => e.EarlyWithdrawalPenalty).HasPrecision(8, 4);
        });
        modelBuilder.Entity<UserDeposit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.AccruedInterest).HasPrecision(18, 2);
            entity.HasOne(e => e.User).WithMany(u => u.UserDeposits).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.DepositProduct).WithMany().HasForeignKey(e => e.DepositProductId);
            entity.HasQueryFilter(e => !e.User.IsDeleted);
        });
        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasKey(e => new { e.FromCurrency, e.ToCurrency });
            entity.Property(e => e.Rate).HasPrecision(18, 6);
        });
    }
}
