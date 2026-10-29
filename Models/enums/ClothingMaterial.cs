using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Enums;

public enum ClothingMaterial
{
    [Display(Name = "Algodón")]
    Cotton,
    [Display(Name = "Poliester")]
    Polyester,
    [Display(Name = "Lana")]
    Wool,
    [Display(Name = "Seda")]
    Silk,
    [Display(Name = "Lino")]
    Linen,
    [Display(Name = "Denim")]
    Denim,
    [Display(Name = "Piel")]
    Leather,
    [Display(Name = "Nailon")]
    Nylon,
    [Display(Name = "Cachemira")]
    Cashmere,
    [Display(Name = "Licra")]
    Elastane
}