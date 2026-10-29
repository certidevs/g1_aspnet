
using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Enums;

public enum SizeType
{
    [Display(Name = "Letra")]
    Letter,
    [Display(Name = "Numerico")]
    Numeric,
    [Display(Name = "Zapato")]
    Shoe
}