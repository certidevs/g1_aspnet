using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public class Location
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Name { get; set; } = string.Empty; // "Oficina Madrid", "Oficina Barcelona"

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es obligatoria")]
    [StringLength(100, ErrorMessage = "La ciudad no puede superar los 100 caracteres.")]
    public string City { get; set; } = string.Empty;

    // Opcionales: con string? el formulario deja guardar el campo vacío (con string, MVC lo exige).
    [StringLength(10, ErrorMessage = "El código postal no puede superar los 10 caracteres.")]
    [RegularExpression(
        @"^\d{5}$",
        ErrorMessage = "El código postal debe tener exactamente 5 dígitos")]
    public string? PostalCode { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    [RegularExpression(
        @"^[6789]\d{8}$",
        ErrorMessage = "El teléfono debe tener 9 dígitos y comenzar por 6, 7, 8 o 9")]
    public string? Phone { get; set; }

    [StringLength(100, ErrorMessage = "El horario no puede superar los 100 caracteres.")]
    public string? OpeningHours { get; set; } // "L-V 9:00-14:00"

    public bool IsActive { get; set; } = false; // por defecto false



    // relaciones:
    // 1 Location → N ClothingItem (dónde está guardada la prenda)
    public List<ClothingItem> ClothingItems { get; set; } = new();

    // 1 Location → N Donation
    public List<Donation> Donations { get; set; } = new();
}
