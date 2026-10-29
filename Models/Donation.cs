using System;
using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public class Donation
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre del donador es obligatorio")]
    public required string DonorName { get; set; }
    [Required(ErrorMessage = "El email del donador es obligatorio")]
    public required string DonorEmail { get; set; }
    public DateTime DonatedAt { get; set; }
    public string? Notes { get; set; }
    public int LocationId { get; set; }
    public Location? Location {get;set;} = new();
    public List<ClothingItem> ClothingItems {get;set;} = new();
}
