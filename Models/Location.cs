using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public class Location
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Name { get; set; } = string.Empty; // "Oficina Madrid", "Oficina Barcelona"

    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public string OpeningHours { get; set; } = string.Empty; // "L-V 9:00-14:00"

    public bool IsActive { get; set; } = false; // por defecto false

    public List<ClothingItem> ClothingItems { get; set; } = new();

    public List<Donation> Donations { get; set; } = new();


    // relaciones:
    // 1 Location → N ClothingItem (dónde está guardada la prenda)
    // 1 Location → N Donation
}
