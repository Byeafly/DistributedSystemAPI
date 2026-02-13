using DistributedSystemAPI.Models.TradeModels;
using DistributedSystemAPI.Models.UserModels;
using DistributedSystemAPI.Models.VideoGameModels;
using Microsoft.EntityFrameworkCore;

namespace DistributedSystemAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<VideoGame> Video_Games { get; set; } = null!;
        public DbSet<TradeOffer> Trade_Offers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<VideoGame>()
                .Property(g => g.Condition)
                .HasConversion<string>();

            modelBuilder.Entity<TradeOffer>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TradeOffer>()
                .HasOne(t => t.FromUser)
                .WithMany(u => u.SentOffers)
                .HasForeignKey(t => t.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TradeOffer>()
                .HasOne(t => t.ToUser)
                .WithMany(u => u.ReceivedOffers)
                .HasForeignKey(t => t.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TradeOffer>()
                .HasOne(t => t.RequestedGame)
                .WithMany(g => g.RequestedInOffers)
                .HasForeignKey(t => t.RequestedGameId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TradeOffer>()
                .HasOne(t => t.OfferedGame)
                .WithMany(g => g.OfferedInOffers)
                .HasForeignKey(t => t.OfferedGameId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
