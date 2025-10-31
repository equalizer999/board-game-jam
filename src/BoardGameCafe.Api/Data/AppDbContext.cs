using BoardGameCafe.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Game entity configuration
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Publisher).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Complexity).HasPrecision(3, 2);
            entity.Property(e => e.DailyRentalFee).HasPrecision(10, 2);
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Category);
            entity.ToTable(t => t.HasCheckConstraint("CK_Game_CopiesInUse", "[CopiesInUse] <= [CopiesOwned]"));
        });

        // Table entity configuration
        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TableNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.TableNumber).IsUnique();
        });

        // Customer entity configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Reservation entity configuration
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Table)
                .WithMany()
                .HasForeignKey(e => e.TableId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TableId, e.ReservationDate, e.StartTime });
        });
    }
}
