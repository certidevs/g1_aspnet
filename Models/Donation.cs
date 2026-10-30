using System;
using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public class Donation
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre del benefactor es obligatorio")]
    public required string DonorName { get; set; } = string.Empty;
    [Required(ErrorMessage = "El email del benefactor es obligatorio")]
    public required string DonorEmail { get; set; } = string.Empty;
    public DateTime DonatedAt { get; set; }
    public string? Notes { get; set; }
    public int LocationId { get; set; }
    public Location? Location {get;set;} = new();
    public List<ClothingItem> ClothingItems {get;set;} = new();
}
