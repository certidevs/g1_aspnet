using System.ComponentModel.DataAnnotations;
using G1AspNet.Enums;

namespace G1AspNet.Models;

public class ClothingItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    public string? Description { get; set; }
    public ClothingColor Color { get; set; }
    public ClothingMaterial Material { get; set; }
    public SizeType SizeType { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria")]
    [StringLength(10)]
    public string Size { get; set; } = string.Empty;

    public ItemCondition Condition { get; set; }

    public ItemStatus Status { get; set; }

    [StringLength(260, ErrorMessage = "El nombre del fichero no puede superar los 260 caracteres.")]
    public string? ImageFileName { get; set; }

    // N ClothingItem → 1 Category (obligatoria)
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // N ClothingItem → 1 Location (obligatoria: dónde está guardada la prenda)
    public int LocationId { get; set; }
    public Location? Location { get; set; }

    // N ClothingItem → 0..1 Donation (opcional, por eso int?: la prenda puede no venir de una donación)
    public int? DonationId { get; set; }
    public Donation? Donation { get; set; }

    // 1 ClothingItem → N Reservation
    public List<Reservation> Reservations { get; set; } = new();
}
