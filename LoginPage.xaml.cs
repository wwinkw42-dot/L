using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AuthApp
{
    public partial class LoginPage : Page
    {
        // логин -> (пароль, роль)
        private static readonly Dictionary<string, (string Password, string Role)> Users = new()
        {
            ["admin"] = ("admin123", "Администратор"),
            ["user"]  = ("user123",  "Пользователь"),
        };

        public LoginPage() => InitializeComponent();

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login))
            { ErrorText.Text = "Введите логин"; return; }
            if (string.IsNullOrEmpty(password))
            { ErrorText.Text = "Введите пароль"; return; }
            if (!Users.TryGetValue(login, out var data))
            { ErrorText.Text = "Пользователь не найден"; return; }
            if (data.Password != password)
            { ErrorText.Text = "Неверный пароль"; return; }

            NavigationService.Navigate(new WelcomePage(login, data.Role));
        }
    }
}
