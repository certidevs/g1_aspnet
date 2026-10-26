using System;

namespace G1AspNet.Models;

public class Donation
{
    public int Id { get; set; }
    public required string DonorName { get; set; }
    public required string DonorEmail { get; set; }
    public DateTime DonatedAt { get; set; }
    public string? Notes { get; set; }
    public int LocationId { get; set; }
    //Falta la propiedad de navagacion de Location
    //Falta la relacion a muchos con ClothingItem (ClothingItemList)
}
