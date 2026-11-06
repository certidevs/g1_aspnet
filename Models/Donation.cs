using System;
using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public class Donation
{
    public int Id { get; set; }

    // Sin "required": ya tienen valor inicial y la validación la hacen los atributos.
    [Required(ErrorMessage = "El nombre del benefactor es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string DonorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email del benefactor es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    [StringLength(256, ErrorMessage = "El email no puede superar los 256 caracteres.")]
    public string DonorEmail { get; set; } = string.Empty;

    public DateTime DonatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
    public string? Notes { get; set; }

    // N Donation → 1 Location (sede donde se entregó la donación)
    public int LocationId { get; set; }
    public Location? Location { get; set; }

    // 1 Donation → N ClothingItem (prendas que llegaron en esta donación)
    public List<ClothingItem> ClothingItems { get; set; } = new();
}
