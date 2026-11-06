using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using G1AspNet.Models;
using G1AspNet.Enums;

namespace G1AspNet.Data;

/// <summary>Recrea la base de datos desde las entidades y prepara roles y cuentas demo.</summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Proyecto de clase sin migraciones: la base se borra y se vuelve a crear
        // desde las entidades en cada arranque (como create-drop con H2 en Spring).
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var configuration = services.GetRequiredService<IConfiguration>();
        if (!configuration.GetValue("SeedData:Enabled", true))
        {
            return;
        }

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await EnsureRoleAsync(roleManager, RoleNames.User);
        await EnsureRoleAsync(roleManager, RoleNames.Admin);

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await EnsureUserAsync(
            userManager,
            username: "admin",
            email: "admin@g1.local",
            displayName: "Administrador",
            password: "Admin123!",
            role: RoleNames.Admin);
        await EnsureUserAsync(
            userManager,
            username: "user",
            email: "user@g1.local",
            displayName: "Usuario de demo",
            password: "User123!",
            role: RoleNames.User);

        // Crear Categories demo
        var random = new Random(1234);
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Pantalones" },
                new Category { Name = "Camisas" },
                new Category { Name = "Camisetas" },
                new Category { Name = "Polos" },
                new Category { Name = "Americanas" },
                new Category { Name = "Calzado" }
                );
            context.SaveChanges();
        }

        // -----------------------------------------
        // 2. UBICACIONES
        // -----------------------------------------

        if (!await context.Locations.AnyAsync())
        {
            context.Locations.AddRange(
                new Location
                {
                    Name = "Oficina Madrid",
                    Address = "Calle Serrano 45",
                    City = "Madrid",
                    PostalCode = "28001",
                    Phone = "910123456",
                    OpeningHours = "Lunes a viernes: 9:00-14:00",
                    IsActive = true
                },
                new Location
                {
                    Name = "Oficina Barcelona",
                    Address = "Carrer de Mallorca 200",
                    City = "Barcelona",
                    PostalCode = "08008",
                    Phone = "930123456",
                    OpeningHours = "Lunes a viernes: 9:00-14:00",
                    IsActive = true
                },
                new Location
                {
                    Name = "Oficina Valencia",
                    Address = "Calle Colón 30",
                    City = "Valencia",
                    PostalCode = "46004",
                    Phone = "960123456",
                    OpeningHours = "Lunes a viernes: 9:00-14:00",
                    IsActive = true
                }
            );

            await context.SaveChangesAsync();
        }

        // -----------------------------------------
        // 3. DONACIONES
        // -----------------------------------------

        if (!await context.Donations.AnyAsync())
        {
            var locations = await context.Locations.ToListAsync();

            string[] names =
            {
                "María García", "Carlos López", "Laura Sánchez",
                "Antonio Martín", "Lucía Fernández", "Javier Pérez",
                "Carmen Ruiz", "Daniel Gómez", "Elena Torres",
                "Miguel Díaz", "Paula Romero", "David Navarro"
            };

            string[] notes =
            {
                "Donación de ropa formal",
                "Ropa de oficina en buen estado",
                "Donación de prendas de temporada",
                "Ropa para entrevistas de trabajo",
                "Varias prendas de vestir",
                "Donación de ropa poco utilizada"
            };

            for (int i = 0; i < 12; i++)
            {
                var donorName = names[i];

                context.Donations.Add(new Donation
                {
                    DonorName = donorName,
                    DonorEmail = $"donante{i + 1}@example.com",
                    DonatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 240)),
                    Notes = notes[random.Next(notes.Length)],
                    LocationId = locations[random.Next(locations.Count)].Id
                });
            }

            await context.SaveChangesAsync();
        }

        // -----------------------------------------
        // 4. PRENDAS
        // -----------------------------------------

        if (!await context.ClothingItems.AnyAsync())
        {
            var categories = await context.Categories.ToListAsync();
            var locations = await context.Locations.ToListAsync();
            var donations = await context.Donations.ToListAsync();

            string[] itemNames =
            {
                "Camisa blanca", "Camisa azul", "Pantalón negro",
                "Pantalón de vestir", "Americana gris", "Americana azul",
                "Camiseta blanca", "Camiseta negra", "Polo azul",
                "Zapatos negros", "Zapatos marrones", "Camisa de rayas",
                "Pantalón beige", "Americana negra", "Polo blanco",
                "Camiseta azul", "Camisa formal", "Pantalón gris",
                "Zapatos formales", "Chaqueta elegante"
            };

            string[] descriptions =
            {
                "Prenda ideal para una entrevista de trabajo.",
                "Prenda en buen estado, adecuada para oficina.",
                "Ropa formal para ocasiones profesionales.",
                "Prenda cómoda y versátil.",
                "Prenda revisada y lista para su uso."
            };

            string[] sizes =
            {
                "XS", "S", "M", "L", "XL",
                "38", "40", "42", "44", "46", "48"
            };

            var colors = Enum.GetValues<ClothingColor>();
            var materials = Enum.GetValues<ClothingMaterial>();
            var sizeTypes = Enum.GetValues<SizeType>();
            var conditions = Enum.GetValues<ItemCondition>();
            var statuses = Enum.GetValues<ItemStatus>();

            for (int i = 0; i < 35; i++)
            {
                // Una de cada cinco prendas no viene de ninguna donación (DonationId = NULL),
                // por ejemplo ropa que compra la propia asociación.
                Donation? donation = i % 5 == 4 ? null : donations[random.Next(donations.Count)];

                context.ClothingItems.Add(new ClothingItem
                {
                    Name = itemNames[random.Next(itemNames.Length)],
                    Description = descriptions[random.Next(descriptions.Length)],

                    Color = colors[random.Next(colors.Length)],
                    Material = materials[random.Next(materials.Length)],
                    SizeType = sizeTypes[random.Next(sizeTypes.Length)],
                    Size = sizes[random.Next(sizes.Length)],

                    Condition = conditions[random.Next(conditions.Length)],
                    Status = statuses[random.Next(statuses.Length)],

                    CategoryId = categories[random.Next(categories.Count)].Id,

                    // La prenda se guarda en la ubicación donde se recibió su donación;
                    // si no tiene donación, en una ubicación cualquiera.
                    LocationId = donation?.LocationId ?? locations[random.Next(locations.Count)].Id,
                    DonationId = donation?.Id
                });
            }

            await context.SaveChangesAsync();
        }

        // -----------------------------------------
        // 5. RESERVAS
        // -----------------------------------------

        if (!await context.Reservations.AnyAsync())
        {
            var demoUser = await userManager.FindByNameAsync("user");

            var clothingItems = await context.ClothingItems.ToListAsync();

            if (demoUser != null && clothingItems.Count > 0)
            {
                var selectedItems = clothingItems
                    .OrderBy(x => random.Next())
                    .Take(Math.Min(8, clothingItems.Count))
                    .ToList();

                // Se reparten los estados de reserva para que haya ejemplos de todos.
                var reservationStatuses = Enum.GetValues<ReservationStatus>();
                var reservationNumber = 0;

                foreach (var item in selectedItems)
                {
                    var status = reservationStatuses[reservationNumber % reservationStatuses.Length];
                    reservationNumber++;

                    // El estado de la prenda sigue al de su reserva.
                    item.Status = status switch
                    {
                        ReservationStatus.Pending or ReservationStatus.Confirmed => ItemStatus.Reserved,
                        ReservationStatus.PickedUp => ItemStatus.Delivered,
                        _ => ItemStatus.Available
                    };

                    // Las reservas vivas (pendientes o confirmadas) son recientes para que no hayan caducado ya.
                    var daysAgo = status is ReservationStatus.Pending or ReservationStatus.Confirmed
                        ? random.Next(1, 5)
                        : random.Next(1, 60);
                    var createdAt = DateTime.UtcNow.AddDays(-daysAgo);
                    var pickupDate = createdAt.AddDays(random.Next(1, 8));

                    context.Reservations.Add(new Reservation
                    {
                        ApplicationUserId = demoUser.Id,
                        ClothingItemId = item.Id,

                        Status = status,
                        CreatedAt = createdAt,
                        PickupDate = pickupDate,
                        ExpiresAt = pickupDate.AddDays(7),

                        Notes = "Reserva de demostración"
                    });
                }

                await context.SaveChangesAsync();
            }
        }

        Console.WriteLine("Datos de demostración comprobados.");
    }


    

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo crear el rol {roleName}.");
            }
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string username,
        string email,
        string displayName,
        string password,
        string role)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                DisplayName = displayName,
                EmailConfirmed = true,
                IsActive = true
            };
            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo crear el usuario demo {username}.");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo asignar el rol {role} a {username}.");
            }
        }
    }
}
