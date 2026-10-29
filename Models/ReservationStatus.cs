using System.ComponentModel.DataAnnotations;

namespace G1AspNet.Models;

public enum ReservationStatus
{
        
        [Display( Name = "Pendiente")]
        Pending,
        [Display( Name = "Confirmado")]
        Confirmed,
        [Display( Name = "Cogida")]
        PickedUp,
        [Display( Name = "Cancelada")]
        Cancelled,
        [Display( Name = "Expirada")]
        Expired




}
