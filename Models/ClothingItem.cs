using System.ComponentModel.DataAnnotations;
using G1AspNet.Enums;

namespace G1AspNet.Models;

public class ClothingItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public ClothingColor Color { get; set; }
    public ClothingMaterial Material { get; set; }
    public SizeType SizeType { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria")]
    [StringLength(10)]
    public string Size { get; set; } = string.Empty;

    public ItemCondition Condition { get; set; }

    public ItemStatus Status { get; set; }

    public string? ImageFileName { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int? DonationId { get; set; }
    public Donation? Donation { get; set; }

    public List<Reservation> Reservations { get; set; } = new();
}
