using ShoesShop.ViewModels;
using ShoesShop.Views;
using System.Windows.Controls;

namespace ShoesShop.Service
{
    public static class TopBarNavigation
    {
        public static void HandleLogin(Page page, UserSession session)
        {
            page.NavigationService.Navigate(new LoginPage(session));
        }
        public static void HandleLogout(Page page, UserSession session)
        {
            session.SignOut();

            page.NavigationService.Navigate(new CatalogPage(session));
            while (page.NavigationService.CanGoBack)
                page.NavigationService.RemoveBackEntry();
        }
    }
}
