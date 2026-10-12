using System.Windows.Controls;

namespace AuthApp
{
    public partial class WelcomePage : Page
    {
        public WelcomePage(string login, string role)
        {
            InitializeComponent();
            WelcomeText.Text = $"Добро пожаловать, {login}, ваша роль - {role}";
        }
    }
}
