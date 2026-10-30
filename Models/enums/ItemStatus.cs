using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Enums;

public enum ItemStatus
{
    [Display(Name = "Disponible")]
    Available,
    [Display(Name = "Reservado")]
    Reserved,
    [Display(Name = "Entregada")]
    Delivered,
    [Display(Name = "Retirada")]
    Retired
}
