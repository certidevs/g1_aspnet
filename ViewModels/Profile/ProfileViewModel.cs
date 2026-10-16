using G1AspNet.Models;

namespace G1AspNet.ViewModels.Profile;

/// <summary>Datos de la página de perfil. Las futuras relaciones del dominio se añadirán aquí.</summary>
public sealed class ProfileViewModel
{
    public ApplicationUser User { get; init; } = null!;
}
