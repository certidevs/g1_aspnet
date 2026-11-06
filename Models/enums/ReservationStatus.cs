using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Enums;
public enum ReservationStatus
{
        
        [Display( Name = "Pendiente")]
        Pending,
        [Display( Name = "Confirmada")]
        Confirmed,
        [Display( Name = "Recogida")]
        PickedUp,
        [Display( Name = "Cancelada")]
        Cancelled,
        [Display( Name = "Expirada")]
        Expired
}
