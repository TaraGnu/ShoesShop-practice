using ShoesShop.Models;

namespace ShoesShop.ViewModels
{
    public class UserSession
    {
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser is not null;
        public UserRole Role { get; private set; } = UserRole.Client;
        public bool IsManager => CurrentUser?.RoleId == (int)UserRole.Manager;
        public bool IsAdmin => CurrentUser?.RoleId == (int)UserRole.Admin;
        public bool IsStaff => IsManager || IsAdmin;
        public void SignIn(User user) => CurrentUser = user;
        public void SignOut() => CurrentUser = null;
    }
}
