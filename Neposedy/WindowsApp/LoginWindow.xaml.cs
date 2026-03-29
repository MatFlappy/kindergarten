using Neposedy;
using Neposedy.Model;
using System;
using System.Linq;
using System.Windows;

namespace NeposedyApp.Windows
{
    public partial class LoginWindow : Window
    {
        public static Users CurrentUser;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = tbLogin.Text.Trim();
                string password = pbPassword.Password.Trim();

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Users user = App.context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CurrentUser = user;

                if (user.Role == "Admin")
                {
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                }
                else if (user.Role == "Teacher")
                {
                    TeacherWindow teacherWindow = new TeacherWindow();
                    teacherWindow.Show();
                }
                else if (user.Role == "Parent")
                {
                    ParentWindow parentWindow = new ParentWindow();
                    parentWindow.Show();
                }
                else
                {
                    MessageBox.Show("Неизвестная роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при входе: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}