using ShoesShop.Models;

namespace ShoesShop.Service
{
    public static class RoleMapper
    {
        public static UserRole FromDbName(string? name) => name switch
        {
            "Администратор" => UserRole.Admin,
            "Менеджер" => UserRole.Manager,
            "Авторизованный пользователь" => UserRole.Client,
            _ => UserRole.Client
        };

        public static string ToDbName(UserRole role) => role switch
        {
            UserRole.Admin => "Администратор",
            UserRole.Manager => "Менеджер",
            UserRole.Client => "Авторизованный пользователь",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };
    }
}
