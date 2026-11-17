using System.ComponentModel.DataAnnotations;
using G1AspNet.Enums;
namespace G1AspNet.Models;

public class Reservation
{
    public int Id { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime PickupDate { get; set; }
    public DateTime ExpiresAt { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    // N Reservation → 1 ApplicationUser (quién reserva)
    [Required (ErrorMessage="El usuario es obligatorio")]
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? ApplicationUser { get; set; }

    // N Reservation → 1 ClothingItem (qué prenda se reserva)
    [Required (ErrorMessage="La prenda es obligatoria")]
    public int ClothingItemId { get; set; }
    public ClothingItem? ClothingItem { get; set; }
}
