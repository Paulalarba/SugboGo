using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SugboGo.Models;

namespace SugboGo.Data;

public sealed class SugboGoDbContext : DbContext
{
    public SugboGoDbContext(DbContextOptions<SugboGoDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<TravelPreferenceRecord> TravelPreferences => Set<TravelPreferenceRecord>();
    public DbSet<DestinationPost> DestinationPosts => Set<DestinationPost>();
    public DbSet<SavedGem> SavedGems => Set<SavedGem>();
    public DbSet<AdminGem> AdminGems => Set<AdminGem>();
    public DbSet<ItineraryTemplate> ItineraryTemplates => Set<ItineraryTemplate>();
    public DbSet<AdminPartner> AdminPartners => Set<AdminPartner>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<TravelSpot> TravelSpots => Set<TravelSpot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DestinationPost -> PostComment as owned types or a separate table.
        // For simplicity and since they are closely tied, I'll use a separate table with a relationship.
        modelBuilder.Entity<DestinationPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsMany(e => e.CommentsList, comments =>
            {
                comments.WithOwner().HasForeignKey("DestinationPostId");
                comments.HasKey("Id");
            });
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasMany(e => e.Bookings)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.SavedGems)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.DestinationPosts)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.TravelPreferences)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TravelSpot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Region);

            entity.HasMany(e => e.Bookings)
                .WithOne(e => e.TravelSpot)
                .HasForeignKey(e => e.TravelSpotId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.SavedGems)
                .WithOne(e => e.TravelSpot)
                .HasForeignKey(e => e.TravelSpotId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.DestinationPosts)
                .WithOne(e => e.TravelSpot)
                .HasForeignKey(e => e.TravelSpotId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TravelPreferenceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);

            ConfigurePreferenceList(entity.Property(e => e.PlaceInterests));
            ConfigurePreferenceList(entity.Property(e => e.ActivityInterests));
        });

        modelBuilder.Entity<SavedGem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TravelSpotId);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TravelSpotId);
        });

        modelBuilder.Entity<AdminGem>().HasKey(e => e.Id);
        modelBuilder.Entity<ItineraryTemplate>().HasKey(e => e.Id);
        modelBuilder.Entity<AdminPartner>().HasKey(e => e.Id);
    }

    private static void ConfigurePreferenceList(Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<List<string>> property)
    {
        property
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
    }
}
