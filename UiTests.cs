using System;
using System.IO;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Input;
using FlaUI.UIA3;
using FluentAssertions;
using Xunit;

namespace AuthApp.Tests
{
    public class UiTests : IDisposable
    {
        private readonly Application _app;
        private readonly UIA3Automation _automation;
        private readonly Window _window;

        public UiTests()
        {
            string exe = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
                @"..\..\..\..\AuthApp\bin\Debug\net8.0-windows\AuthApp.exe"));
            _app = Application.Launch(exe);
            _automation = new UIA3Automation();
            _window = _app.GetMainWindow(_automation);
        }

        // 5.2.3 — закрыть приложение и освободить ресурсы
        public void Dispose()
        {
            _app.Close();
            _app.Dispose();
            _automation.Dispose();
        }

        // ---------- вспомогательные методы ----------
        private AutomationElement ById(string id) =>
            _window.FindFirstDescendant(_window.ConditionFactory.ByAutomationId(id));

        private AutomationElement ByText(string text) =>
            _window.FindFirstDescendant(_window.ConditionFactory.ByText(text));

        private void Type(AutomationElement el, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            el.Focus();
            Keyboard.Type(text);
        }

        private void Shot(string name)
        {
            string dir = Path.Combine(AppContext.BaseDirectory, "Screenshots");
            Directory.CreateDirectory(dir);
            Capture.Element(_window).ToFile(Path.Combine(dir, name + ".png"));
        }

        // ---------- тесты ----------

        // Способ 1: AutomationProperties.AutomationId (LoginTextBox, LoginButton)
        [Fact]
        public void Admin_ValidCredentials_ShowsWelcome()
        {
            Type(ById("LoginTextBox"), "admin");
            Type(ById("PasswordBox"), "admin123");
            ById("LoginButton").AsButton().Click();

            string text = ById("WelcomeText").Name;
            Shot("01_admin_ok");
            text.Should().Be("Добро пожаловать, admin, ваша роль - Администратор");
        }

        // Способ 3: поиск по тексту на элементе (кнопка «Войти»)
        [Fact]
        public void User_ValidCredentials_ShowsWelcome()
        {
            Type(ById("LoginTextBox"), "user");
            Type(ById("PasswordBox"), "user123");
            ByText("Войти").AsButton().Click();

            string text = ById("WelcomeText").Name;
            Shot("02_user_ok");
            text.Should().Be("Добро пожаловать, user, ваша роль - Пользователь");
        }

        // Способ 2: x:Name (ErrorText, PasswordBox — у них задан только x:Name)
        [Fact]
        public void EmptyLogin_ShowsError()
        {
            Type(ById("PasswordBox"), "admin123");
            ById("LoginButton").AsButton().Click();

            Shot("03_empty_login");
            ById("ErrorText").Name.Should().Be("Введите логин");
        }

        [Fact]
        public void EmptyPassword_ShowsError()
        {
            Type(ById("LoginTextBox"), "admin");
            ByText("Войти").AsButton().Click();

            Shot("04_empty_password");
            ById("ErrorText").Name.Should().Be("Введите пароль");
        }

        [Fact]
        public void UnknownLogin_ShowsError()
        {
            Type(ById("LoginTextBox"), "hacker");
            Type(ById("PasswordBox"), "12345");
            ById("LoginButton").AsButton().Click();

            Shot("05_unknown_login");
            ById("ErrorText").Name.Should().Be("Пользователь не найден");
        }

        [Fact]
        public void WrongPassword_ShowsError()
        {
            Type(ById("LoginTextBox"), "admin");
            Type(ById("PasswordBox"), "wrong");
            ById("LoginButton").AsButton().Click();

            Shot("06_wrong_password");
            ById("ErrorText").Name.Should().Be("Неверный пароль");
        }

        [Fact]
        public void WrongPassword_DoesNotNavigateToWelcomePage()
        {
            Type(ById("LoginTextBox"), "user");
            Type(ById("PasswordBox"), "bad");
            ById("LoginButton").AsButton().Click();

            Shot("07_no_navigation");
            ById("WelcomeText").Should().BeNull();
        }
    }
}
