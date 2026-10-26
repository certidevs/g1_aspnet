using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using G1AspNet.Data;


namespace RestaurantesAspNet.Controllers;

public class LocationController : Controller
{
    private readonly ApplicationDbContext context;

    public LocationController(ApplicationDbContext context)
    {
        this.context = context;
    }


    

}