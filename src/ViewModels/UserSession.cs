using ShoesShop.Models;

namespace ShoesShop.ViewModels
{
    public class UserSession
    {
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser is not null;


        public void SignIn(User user) => CurrentUser = user;
        public void SignOut() => CurrentUser = null;
    }
}
