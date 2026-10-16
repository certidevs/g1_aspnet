using G1AspNet.Models;

namespace G1AspNet.ViewModels.Users;

/// <summary>Una fila del listado administrativo de cuentas.</summary>
public sealed class UserListItemViewModel
{
    public ApplicationUser User { get; init; } = null!;
    public string Role { get; init; } = RoleNames.User;
}
