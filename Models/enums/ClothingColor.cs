
using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Enums;

public enum ClothingColor
{
    [Display(Name = "Negro")]
    Black,
    [Display(Name = "Blanco")]
    White,
    [Display(Name = "Gris")]
    Grey,
    [Display(Name = "Azul")]
    Blue,
    [Display(Name = "Rojo")]
    Red,
    [Display(Name = "Verde")]
    Green,
    [Display(Name = "Marron")]
    Brown,
    [Display(Name = "Beis")]
    Beige,
    [Display(Name = "Otro")]
    Other,
}