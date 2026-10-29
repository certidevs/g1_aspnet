namespace G1AspNet.Enums;
using System.ComponentModel.DataAnnotations;

public enum ItemStatus
{

        [Display( Name = "Disponible")]
        Available,
        [Display( Name = "Reservado")]
        Reserved,
        [Display( Name = "Entregada")]
        Delivered,
        [Display( Name = "Retirada")]
        Retired

}
