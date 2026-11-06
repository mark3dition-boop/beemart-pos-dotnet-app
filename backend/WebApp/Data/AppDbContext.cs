using Microsoft.EntityFrameworkCore;
using BeeMartAPI.Models;

namespace BeeMartAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<MsUser> MsUsers { get; set; }
        public DbSet<MsProduct> MsProducts { get; set; }
        public DbSet<LtPaymentMethod> LtPaymentMethods { get; set; }
        public DbSet<TrTransaction> TrTransactions { get; set; }
        public DbSet<TrTransactionDetail> TrTransactionDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mendaftarkan / Memastikan Relasi Foreign Key
            modelBuilder.Entity<TrTransaction>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.IdUser)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrTransaction>()
                .HasOne(t => t.PaymentMethod)
                .WithMany()
                .HasForeignKey(t => t.IdPaymentMethod)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrTransactionDetail>()
                .HasOne(td => td.Transaction)
                .WithMany(t => t.TransactionDetails)
                .HasForeignKey(td => td.IdTransaction)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrTransactionDetail>()
                .HasOne(td => td.Product)
                .WithMany()
                .HasForeignKey(td => td.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}