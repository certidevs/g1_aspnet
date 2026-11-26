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

            var madrid = context.Locations.First(l => l.Name == "Oficina Madrid");
            var barcelona = context.Locations.First(l => l.Name == "Oficina Barcelona");
            var valencia = context.Locations.First(l => l.Name == "Oficina Valencia");

            context.Donations.AddRange(
                new Donation
                {
                    DonorName = "Maria del Olmo",
                    DonorEmail = "Dolmo@example.com",
                    DonatedAt = new DateTime(2026, 2, 2),
                    Notes = "Donación de ropa poco utilizada",
                    Location = barcelona
                },
                new Donation
                {
                    DonorName = "Alan Sastre",
                    DonorEmail = "Alan@certidevs.com",
                    DonatedAt = new DateTime(2026, 3, 3),
                    Notes = "Ropa de oficina en buen estado",
                    Location = valencia
                },
                new Donation
                {
                    DonorName = "Elena Nito Del Bosque",
                    DonorEmail = "ElenaBosque@example.com",
                    DonatedAt = new DateTime(2026, 1, 1),
                    Notes = "Donación de ropa formal",
                    Location = madrid
                },
                new Donation
                {
                    DonorName = "Maria Mucha Gracia",
                    DonorEmail = "MariaGraciosilla@comicos.com",
                    DonatedAt = new DateTime(2026, 4, 4),
                    Notes = "Ropa de oficina, revisar",
                    Location = madrid
                },
                new Donation
                {
                    DonorName = "Luz Cuesta Mucho",
                    DonorEmail = "Iluminada73@example.com",
                    DonatedAt = new DateTime(2026, 5, 5),
                    Notes = "Dona ropa variada, Hay prendas que no valen, llevarlas a cruz roja",
                    Location = barcelona
                },
                new Donation
                {
                    DonorName = "Armando Bronca Segura",
                    DonorEmail = "Cabreadito@example.com",
                    DonatedAt = new DateTime(2026, 6, 6),
                    Notes = "Ropa en perfecto estado",
                    Location = valencia
                },
                new Donation
                {
                    DonorName = "Dolores Fuertes de Barriga",
                    DonorEmail = "LaDolo93@example.com",
                    DonatedAt = new DateTime(2026, 7, 7),
                    Notes = "Casi todo en perfecto estado, una chaqueta hay que mandarla a remendar",
                    Location = madrid
                },
                new Donation
                {
                    DonorName = "Elsa Pato",
                    DonorEmail = "LaDeFrozen@example.com",
                    DonatedAt = new DateTime(2026, 8, 8),
                    Notes = "Ropa principalmente blanca y azul, toda recien salida de la lavanderia, directa al armario",
                    Location = barcelona
                },
                new Donation
                {
                    DonorName = "Mario Neta",
                    DonorEmail = "MarioN@TitiriterosAnonimos.com",
                    DonatedAt = new DateTime(2026, 9, 9),
                    Notes = "Ropa en perfecto estado, se le ha colado un traje de muñeco, llamar para devolverselo",
                    Location = valencia
                },
                new Donation
                {
                    DonorName = "Susana Oria",
                    DonorEmail = "Susan@VeganosUnidos.com",
                    DonatedAt = new DateTime(2026, 10, 10),
                    Notes = "Ropa bastante gastada, revisar",
                    Location = madrid
                });
            await context.SaveChangesAsync();
        }

        // -----------------------------------------
        // 4. PRENDAS
        // -----------------------------------------

        if (!await context.ClothingItems.AnyAsync())
        {
            var madrid = await context.Locations.SingleAsync(l => l.Name == "Oficina Madrid");
            var barcelona = await context.Locations.SingleAsync(l => l.Name == "Oficina Barcelona");
            var valencia = await context.Locations.SingleAsync(l => l.Name == "Oficina Valencia");

            var camisas = await context.Categories.SingleAsync(c => c.Name == "Camisas");
            var pantalones = await context.Categories.SingleAsync(c => c.Name == "Pantalones");
            var americanas = await context.Categories.SingleAsync(c => c.Name == "Americanas");
            var calzado = await context.Categories.SingleAsync(c => c.Name == "Calzado");

            var donacionMaria = await context.Donations.SingleAsync(d => d.DonorName == "Maria del Olmo");
            var donacionAlan = await context.Donations.SingleAsync(d => d.DonorName == "Alan Sastre");

            context.ClothingItems.AddRange(
                new ClothingItem
                {
                    Name = "Camisa blanca",
                    Description = "Camisa formal de manga larga.",
                    Color = ClothingColor.White,
                    Material = ClothingMaterial.Cotton,
                    SizeType = SizeType.Letter,
                    Size = "M",
                    Condition = ItemCondition.LikeNew,
                    Status = ItemStatus.Available,
                    Category = camisas,
                    Location = barcelona,
                    Donation = donacionMaria
                },
                new ClothingItem
                {
                    Name = "Camisa azul",
                    Description = "Camisa de vestir azul claro.",
                    Color = ClothingColor.Blue,
                    Material = ClothingMaterial.Cotton,
                    SizeType = SizeType.Letter,
                    Size = "L",
                    Condition = ItemCondition.Good,
                    Status = ItemStatus.Available,
                    Category = camisas,
                    Location = valencia,
                    Donation = donacionAlan
                },
                new ClothingItem
                {
                    Name = "Pantalón negro",
                    Description = "Pantalón de vestir clásico.",
                    Color = ClothingColor.Black,
                    Material = ClothingMaterial.Polyester,
                    SizeType = SizeType.Numeric,
                    Size = "42",
                    Condition = ItemCondition.LikeNew,
                    Status = ItemStatus.Available,
                    Category = pantalones,
                    Location = valencia,
                    Donation = donacionAlan
                },
                new ClothingItem
                {
                    Name = "Americana gris",
                    Description = "Americana de traje en buen estado.",
                    Color = ClothingColor.Grey,
                    Material = ClothingMaterial.Wool,
                    SizeType = SizeType.Letter,
                    Size = "M",
                    Condition = ItemCondition.LikeNew,
                    Status = ItemStatus.Available,
                    Category = americanas,
                    Location = madrid,
                    Donation = donacionMaria
                },
                new ClothingItem
                {
                    Name = "Zapatos negros",
                    Description = "Zapatos formales de piel.",
                    Color = ClothingColor.Black,
                    Material = ClothingMaterial.Leather,
                    SizeType = SizeType.Shoe,
                    Size = "42",
                    Condition = ItemCondition.Good,
                    Status = ItemStatus.Available,
                    Category = calzado,
                    Location = madrid,
                    Donation = null
                }
            );

            await context.SaveChangesAsync();
        }
        // -----------------------------------------
        // 5. RESERVAS
        // -----------------------------------------



        if (!await context.Reservations.AnyAsync())
        {
            var demoUser = await userManager.FindByNameAsync("user");

            if (demoUser != null)
            {
                var camisaBlanca = await context.ClothingItems
                    .SingleAsync(i => i.Name == "Camisa blanca");

                var pantalonNegro = await context.ClothingItems
                    .SingleAsync(i => i.Name == "Pantalón negro");

                var americanaGris = await context.ClothingItems
                    .SingleAsync(i => i.Name == "Americana gris");

                camisaBlanca.Status = ItemStatus.Reserved;
                pantalonNegro.Status = ItemStatus.Reserved;
                americanaGris.Status = ItemStatus.Delivered;

                context.Reservations.AddRange(
                    new Reservation
                    {
                        ApplicationUserId = demoUser.Id,
                        ClothingItem = camisaBlanca,
                        Status = ReservationStatus.Pending,
                        CreatedAt = new DateTime(2026, 11, 24),
                        PickupDate = new DateTime(2026, 11, 28),
                        ExpiresAt = new DateTime(2026, 12, 5),
                        Notes = "Reserva para una entrevista de trabajo."
                    },
                    new Reservation
                    {
                        ApplicationUserId = demoUser.Id,
                        ClothingItem = pantalonNegro,
                        Status = ReservationStatus.Confirmed,
                        CreatedAt = new DateTime(2026, 11, 20),
                        PickupDate = new DateTime(2026, 11, 29),
                        ExpiresAt = new DateTime(2026, 12, 6),
                        Notes = "Conjunto para una entrevista presencial."
                    },
                    new Reservation
                    {
                        ApplicationUserId = demoUser.Id,
                        ClothingItem = americanaGris,
                        Status = ReservationStatus.PickedUp,
                        CreatedAt = new DateTime(2026, 10, 10),
                        PickupDate = new DateTime(2026, 10, 15),
                        ExpiresAt = new DateTime(2026, 10, 22),
                        Notes = "Prenda recogida en la oficina."
                    }
                );

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
