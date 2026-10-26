using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Relación con ClothingItem
    // public List<ClothigItem> ClothigItems { get; set; } = new();
}