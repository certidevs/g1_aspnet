using System.ComponentModel.DataAnnotations;
using G1AspNet.Enums;

namespace G1AspNet.Models;

public class ClothingItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El color es obligatorio")]
    public ClothingColor Color { get; set; }

    [Required(ErrorMessage = "El material es obligatorio")]
    public ClothingMaterial Material { get; set; } 

    [Required(ErrorMessage = "El tipo de talla es obligatorio")]
    public SizeType SizeType {get; set; }
     
    [Required(ErrorMessage = "La talla es obligatoria")]
    public string Size { get; set; } = string.Empty;

    // Condition Enum

    // Status Enum

    public string? ImageFileName { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category;
    
    public int LocationId { get; set; }
    public Location? Location;

    public int? DonationId { get; set; }
    public Donation? Donation;
}