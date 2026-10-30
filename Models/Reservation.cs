using G1AspNet.Enums;
namespace G1AspNet.Models;

public class Reservation
{
    public int Id { get; set; }

    public ReservationStatus ReservationStatus { get; set; } = ReservationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime PickupDate { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? Notes { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int ClothingItemId { get; set; }
    public ClothingItem ClothingItem { get; set; } = null!;
}
