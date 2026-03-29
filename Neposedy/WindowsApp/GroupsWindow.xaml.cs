using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;

namespace Neposedy.WindowsApp
{
    public partial class GroupsWindow : Window
    {
        public class GroupView
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string AgeRangeText { get; set; }
            public string TeacherName { get; set; }
            public string DescriptionText { get; set; }
        }

        public GroupsWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            LoadTeachers();
            LoadGroups();
        }

        private void LoadTeachers()
        {
            var teachers = App.context.Teachers
                .ToList()
                .Select(t => new
                {
                    t.Id,
                    FullName = t.Users != null ? t.Users.FullName : "—"
                })
                .ToList();

            cbTeachers.ItemsSource = teachers;
            cbTeachers.DisplayMemberPath = "FullName";
            cbTeachers.SelectedValuePath = "Id";
        }

        private void LoadGroups()
        {
            var list = App.context.Groups
                .ToList()
                .Select(g => new GroupView
                {
                    Id = g.Id,
                    Name = g.Name,
                    AgeRangeText = string.IsNullOrWhiteSpace(g.AgeRange) ? "—" : g.AgeRange,
                    TeacherName = g.Teachers != null && g.Teachers.Users != null ? g.Teachers.Users.FullName : "—",
                    DescriptionText = string.IsNullOrWhiteSpace(g.Description) ? "—" : g.Description
                })
                .ToList();

            dgGroups.ItemsSource = list;
        }

        private GroupView GetSelectedGroup()
        {
            return dgGroups.SelectedItem as GroupView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbName.Text))
                {
                    MessageBox.Show("Введите название группы.");
                    return;
                }

                Groups group = new Groups()
                {
                    Name = tbName.Text.Trim(),
                    AgeRange = tbAgeRange.Text.Trim(),
                    TeacherId = cbTeachers.SelectedValue != null ? (int?)cbTeachers.SelectedValue : null,
                    Description = tbDescription.Text.Trim()
                };

                App.context.Groups.Add(group);
                App.context.SaveChanges();

                MessageBox.Show("Группа добавлена.");
                LoadGroups();
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
                GroupView selected = GetSelectedGroup();
                if (selected == null)
                {
                    MessageBox.Show("Выберите группу.");
                    return;
                }

                var group = App.context.Groups.FirstOrDefault(g => g.Id == selected.Id);
                if (group == null)
                {
                    MessageBox.Show("Группа не найдена.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(tbName.Text))
                {
                    MessageBox.Show("Введите название группы.");
                    return;
                }

                group.Name = tbName.Text.Trim();
                group.AgeRange = tbAgeRange.Text.Trim();
                group.TeacherId = cbTeachers.SelectedValue != null ? (int?)cbTeachers.SelectedValue : null;
                group.Description = tbDescription.Text.Trim();

                App.context.SaveChanges();
                MessageBox.Show("Данные обновлены.");
                LoadGroups();
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
                GroupView selected = GetSelectedGroup();
                if (selected == null)
                {
                    MessageBox.Show("Выберите группу.");
                    return;
                }

                var group = App.context.Groups.FirstOrDefault(g => g.Id == selected.Id);
                if (group == null)
                {
                    MessageBox.Show("Группа не найдена.");
                    return;
                }

                var result = MessageBox.Show("Удалить выбранную группу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                App.context.Groups.Remove(group);
                App.context.SaveChanges();

                MessageBox.Show("Группа удалена.");
                LoadGroups();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadGroups();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
            Close();
        }

        private void ClearFields()
        {
            tbName.Text = "";
            tbAgeRange.Text = "";
            tbDescription.Text = "";
            cbTeachers.SelectedItem = null;
        }
    }
}