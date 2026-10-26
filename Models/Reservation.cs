using System;

namespace G1AspNet.Models;

public class Reservation
{

    public int Id {get; set;}
    
    public ReservationStatus ReservationStatus {get; set;} = new();
    public DateTime CreatedAt {get; set;} 
    public DateTime PickupDate {get; set;}
    public DateTime ExpiresAt  {get; set;}
    public string? Notes {get; set;} = string.Empty;

    public string UserId {get; set;} = string.Empty;

    public ApplicationUser ApplicationUser {get; set;} = new();

    public int ClothingItemId {get; set;}

    public ClothingItem ClothingItem {get; set;} = new();


}
