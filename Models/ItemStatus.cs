namespace G1AspNet.Models;
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
