using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Enums;

public enum ItemCondition
{
    [Display(Name = "Nuevo")]
    New,
    [Display(Name = "Como nuevo")]
    LikeNew,
    [Display(Name = "Bien")]
    Good,
    [Display(Name = "Desgastado")]
    Worn
}
