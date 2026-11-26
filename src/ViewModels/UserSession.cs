using ShoesShop.Models;

namespace ShoesShop.ViewModels
{
    public class UserSession
    {
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser is not null;
        public string? Role => CurrentUser?.Role.Name;
        public bool IsAdmin => MatchesRole("Администратор", "Administrator", "Admin");
        public bool IsManager => MatchesRole("Менеджер", "Manager");
        public bool IsStaff => IsAdmin || IsManager;
        public void SignIn(User user) => CurrentUser = user;
        public void SignOut() => CurrentUser = null;
        private bool MatchesRole(params string[] names)
        {
            var role = Role;
            if (string.IsNullOrWhiteSpace(role))
                return false;

            var trimmed = role.Trim();
            return names.Any(n => string.Equals(n, trimmed, StringComparison.OrdinalIgnoreCase));
        }
    }
}
