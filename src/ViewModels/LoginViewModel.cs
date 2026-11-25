using Microsoft.EntityFrameworkCore;
using ShoesShop.Commands;
using ShoesShop.Models;
using ShoesShop.Views;

namespace ShoesShop.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly UserSession _session;
        private string _loginInput = string.Empty;
        private string _errorMessage = string.Empty;

        public LoginViewModel(UserSession session) => _session = session;

        public string LoginInput
        {
            get => _loginInput;
            set
            {
                if (SetProperty(ref _loginInput, value))
                    ErrorMessage = string.Empty;
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool Login()
        {
            try
            {
                var user = Authenticate(LoginInput);
                _session.SignIn(user);
                return true;
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Не удалось войти. Техническая информация: {ex.Message}";
            }
            return false;
        }

        private static User Authenticate(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Введите логин — пустое поле недопустимо.", nameof(login));

            var normalized = login.Trim();

            using var context = new ShoesContext();
            var user = context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == normalized);

            if (user is null)
                throw new InvalidOperationException(
                    $"Пользователь с логином «{normalized}» не найден. Проверьте написание.");

            return user;
        }
    }
}
