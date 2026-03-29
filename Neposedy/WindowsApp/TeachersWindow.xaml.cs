using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;

namespace Neposedy.WindowsApp
{
    public partial class TeachersWindow : Window
    {
        public class TeacherView
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string FullName { get; set; }
            public string Login { get; set; }
            public string Phone { get; set; }
            public string Education { get; set; }
            public int? ExperienceYears { get; set; }

            public string PhoneText
            {
                get { return string.IsNullOrWhiteSpace(Phone) ? "—" : Phone; }
            }

            public string EducationText
            {
                get { return string.IsNullOrWhiteSpace(Education) ? "—" : Education; }
            }

            public string ExperienceYearsText
            {
                get { return ExperienceYears.HasValue ? ExperienceYears.Value.ToString() : "—"; }
            }
        }

        public TeachersWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            LoadTeachers();
        }

        private void LoadTeachers()
        {
            var list = App.context.Teachers
                .ToList()
                .Select(t => new TeacherView
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    FullName = t.Users != null ? t.Users.FullName : "",
                    Login = t.Users != null ? t.Users.Login : "",
                    Phone = t.Phone,
                    Education = t.Education,
                    ExperienceYears = t.ExperienceYears
                })
                .ToList();

            dgTeachers.ItemsSource = list;
        }

        private TeacherView GetSelectedTeacher()
        {
            return dgTeachers.SelectedItem as TeacherView;
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

                int? experience = null;
                if (!string.IsNullOrWhiteSpace(tbExperienceYears.Text))
                {
                    int parsed;
                    if (!int.TryParse(tbExperienceYears.Text.Trim(), out parsed))
                    {
                        MessageBox.Show("Стаж должен быть числом.");
                        return;
                    }
                    experience = parsed;
                }

                Users newUser = new Users()
                {
                    FullName = tbFullName.Text.Trim(),
                    Login = tbLogin.Text.Trim(),
                    Password = tbPassword.Text.Trim(),
                    Role = "Teacher"
                };

                App.context.Users.Add(newUser);
                App.context.SaveChanges();

                Teachers newTeacher = new Teachers()
                {
                    UserId = newUser.Id,
                    Phone = tbPhone.Text.Trim(),
                    Education = tbEducation.Text.Trim(),
                    ExperienceYears = experience
                };

                App.context.Teachers.Add(newTeacher);
                App.context.SaveChanges();

                MessageBox.Show("Воспитатель добавлен.");
                LoadTeachers();
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
                TeacherView selected = GetSelectedTeacher();
                if (selected == null)
                {
                    MessageBox.Show("Выберите воспитателя.");
                    return;
                }

                var teacher = App.context.Teachers.FirstOrDefault(t => t.Id == selected.Id);
                if (teacher == null)
                {
                    MessageBox.Show("Воспитатель не найден.");
                    return;
                }

                var user = App.context.Users.FirstOrDefault(u => u.Id == teacher.UserId);
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

                int? experience = null;
                if (!string.IsNullOrWhiteSpace(tbExperienceYears.Text))
                {
                    int parsed;
                    if (!int.TryParse(tbExperienceYears.Text.Trim(), out parsed))
                    {
                        MessageBox.Show("Стаж должен быть числом.");
                        return;
                    }
                    experience = parsed;
                }

                user.FullName = tbFullName.Text.Trim();
                user.Login = tbLogin.Text.Trim();

                if (!string.IsNullOrWhiteSpace(tbPassword.Text))
                {
                    user.Password = tbPassword.Text.Trim();
                }

                teacher.Phone = tbPhone.Text.Trim();
                teacher.Education = tbEducation.Text.Trim();
                teacher.ExperienceYears = experience;

                App.context.SaveChanges();

                MessageBox.Show("Данные обновлены.");
                LoadTeachers();
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
                TeacherView selected = GetSelectedTeacher();
                if (selected == null)
                {
                    MessageBox.Show("Выберите воспитателя.");
                    return;
                }

                var teacher = App.context.Teachers.FirstOrDefault(t => t.Id == selected.Id);
                if (teacher == null)
                {
                    MessageBox.Show("Воспитатель не найден.");
                    return;
                }

                var user = App.context.Users.FirstOrDefault(u => u.Id == teacher.UserId);

                var result = MessageBox.Show("Удалить выбранного воспитателя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                App.context.Teachers.Remove(teacher);
                App.context.SaveChanges();

                if (user != null)
                {
                    App.context.Users.Remove(user);
                    App.context.SaveChanges();
                }

                MessageBox.Show("Воспитатель удален.");
                LoadTeachers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTeachers();
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
            tbEducation.Text = "";
            tbExperienceYears.Text = "";
        }
    }
}