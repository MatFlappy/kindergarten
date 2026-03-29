using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;

namespace Neposedy.WindowsApp
{
    public partial class ParentsWindow : Window
    {
        public class ParentView
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string FullName { get; set; }
            public string Login { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }

            public string PhoneText
            {
                get { return string.IsNullOrWhiteSpace(Phone) ? "—" : Phone; }
            }

            public string EmailText
            {
                get { return string.IsNullOrWhiteSpace(Email) ? "—" : Email; }
            }

            public string AddressText
            {
                get { return string.IsNullOrWhiteSpace(Address) ? "—" : Address; }
            }
        }

        public ParentsWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            LoadParents();
        }

        private void LoadParents()
        {
            var list = App.context.Parents
                .ToList()
                .Select(p => new ParentView
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    FullName = p.Users != null ? p.Users.FullName : "",
                    Login = p.Users != null ? p.Users.Login : "",
                    Phone = p.Phone,
                    Email = p.Email,
                    Address = p.Address
                })
                .ToList();

            dgParents.ItemsSource = list;
        }

        private ParentView GetSelectedParent()
        {
            return dgParents.SelectedItem as ParentView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbFullName.Text) ||
                    string.IsNullOrWhiteSpace(tbLogin.Text) ||
                    string.IsNullOrWhiteSpace(tbPassword.Text))
                {
                    MessageBox.Show("Заполните ФИО, логин и пароль.");
                    return;
                }

                var existingUser = App.context.Users.FirstOrDefault(u => u.Login == tbLogin.Text.Trim());
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }

                Users newUser = new Users()
                {
                    FullName = tbFullName.Text.Trim(),
                    Login = tbLogin.Text.Trim(),
                    Password = tbPassword.Text.Trim(),
                    Role = "Parent"
                };

                App.context.Users.Add(newUser);
                App.context.SaveChanges();

                Parents newParent = new Parents()
                {
                    UserId = newUser.Id,
                    Phone = tbPhone.Text.Trim(),
                    Email = tbEmail.Text.Trim(),
                    Address = tbAddress.Text.Trim()
                };

                App.context.Parents.Add(newParent);
                App.context.SaveChanges();

                MessageBox.Show("Родитель добавлен.");
                LoadParents();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentView selected = GetSelectedParent();
                if (selected == null)
                {
                    MessageBox.Show("Выберите родителя.");
                    return;
                }

                var parent = App.context.Parents.FirstOrDefault(p => p.Id == selected.Id);
                if (parent == null)
                {
                    MessageBox.Show("Родитель не найден.");
                    return;
                }

                var user = App.context.Users.FirstOrDefault(u => u.Id == parent.UserId);
                if (user == null)
                {
                    MessageBox.Show("Учетная запись не найдена.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(tbFullName.Text) ||
                    string.IsNullOrWhiteSpace(tbLogin.Text))
                {
                    MessageBox.Show("Заполните ФИО и логин.");
                    return;
                }

                var existingUser = App.context.Users.FirstOrDefault(u => u.Login == tbLogin.Text.Trim() && u.Id != user.Id);
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }

                user.FullName = tbFullName.Text.Trim();
                user.Login = tbLogin.Text.Trim();

                if (!string.IsNullOrWhiteSpace(tbPassword.Text))
                {
                    user.Password = tbPassword.Text.Trim();
                }

                parent.Phone = tbPhone.Text.Trim();
                parent.Email = tbEmail.Text.Trim();
                parent.Address = tbAddress.Text.Trim();

                App.context.SaveChanges();

                MessageBox.Show("Данные обновлены.");
                LoadParents();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка редактирования: " + ex.Message);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ParentView selected = GetSelectedParent();
                if (selected == null)
                {
                    MessageBox.Show("Выберите родителя.");
                    return;
                }

                var parent = App.context.Parents.FirstOrDefault(p => p.Id == selected.Id);
                if (parent == null)
                {
                    MessageBox.Show("Родитель не найден.");
                    return;
                }

                var user = App.context.Users.FirstOrDefault(u => u.Id == parent.UserId);

                var result = MessageBox.Show("Удалить выбранного родителя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                App.context.Parents.Remove(parent);
                App.context.SaveChanges();

                if (user != null)
                {
                    App.context.Users.Remove(user);
                    App.context.SaveChanges();
                }

                MessageBox.Show("Родитель удален.");
                LoadParents();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadParents();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
            Close();
        }

        private void ClearFields()
        {
            tbFullName.Text = "";
            tbLogin.Text = "";
            tbPassword.Text = "";
            tbPhone.Text = "";
            tbEmail.Text = "";
            tbAddress.Text = "";
        }
    }
}