using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    public string? Description { get; set; }

    // Relación con ClothingItem
    public List<ClothingItem> ClothingItems { get; set; } = new();
}