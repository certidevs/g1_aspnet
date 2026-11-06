using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using G1AspNet.Models;

namespace G1AspNet.Data;

/// <summary>
/// Contexto de EF Core: configura SQLite y reúne las tablas de la aplicación.
/// Las entidades del dominio del proyecto se añadirán aquí.
/// </summary>
public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>EF Core recibe estas opciones desde Program.cs para conectar con SQLite.</summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    // dbSet
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ClothingItem> ClothingItems => Set<ClothingItem>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<Reservation> Reservations => Set<Reservation>();


    /// <summary>
    /// Configura las columnas añadidas a la tabla de usuarios de Identity y
    /// las relaciones entre entidades, con su comportamiento de borrado explícito.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.DisplayName).HasMaxLength(100);
            entity.Property(user => user.AvatarFileName).HasMaxLength(260);
            entity.Property(user => user.CreatedAt).IsRequired();
        });

        // Regla de borrado (OnDelete) de cada relación:
        //   Cascade  -> el hijo no tiene sentido sin el padre.
        //   Restrict -> historial o dato importante: no se puede borrar el padre si tiene hijos.
        //   SetNull  -> solo con clave foránea opcional (int?): el hijo se queda sin padre.

        // Category 1:N ClothingItem. Restrict: una categoría con prendas no se puede borrar (el catálogo es un dato importante).
        modelBuilder.Entity<ClothingItem>()
            .HasOne(item => item.Category)
            .WithMany(category => category.ClothingItems)
            .HasForeignKey(item => item.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Location 1:N ClothingItem. Restrict: una sede con prendas guardadas no se puede borrar (primero hay que moverlas).
        modelBuilder.Entity<ClothingItem>()
            .HasOne(item => item.Location)
            .WithMany(location => location.ClothingItems)
            .HasForeignKey(item => item.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Donation 1:N ClothingItem (opcional). SetNull: si se borra la donación, la prenda física sigue existiendo y su DonationId pasa a NULL.
        modelBuilder.Entity<ClothingItem>()
            .HasOne(item => item.Donation)
            .WithMany(donation => donation.ClothingItems)
            .HasForeignKey(item => item.DonationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Location 1:N Donation. Restrict: las donaciones son historial; una sede con donaciones no se puede borrar.
        modelBuilder.Entity<Donation>()
            .HasOne(donation => donation.Location)
            .WithMany(location => location.Donations)
            .HasForeignKey(donation => donation.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // ClothingItem 1:N Reservation. Restrict: las reservas son historial; una prenda con reservas no se borra (se marca como Retired).
        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.ClothingItem)
            .WithMany(item => item.Reservations)
            .HasForeignKey(reservation => reservation.ClothingItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // ApplicationUser 1:N Reservation. Restrict: toda relación con ApplicationUser lo es; un usuario con reservas se desactiva (IsActive), no se borra.
        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.ApplicationUser)
            .WithMany(user => user.Reservations)
            .HasForeignKey(reservation => reservation.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
