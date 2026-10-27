using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public class ClothingItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Name { get; set; } = string.Empty;

    // Color Enum

    // Material Enum

    // SizeType Enum
     
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