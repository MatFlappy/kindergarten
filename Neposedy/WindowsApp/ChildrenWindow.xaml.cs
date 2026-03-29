using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;

namespace Neposedy.WindowsApp
{
    public partial class ChildrenWindow : Window
    {
        public class ChildView
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public DateTime BirthDate { get; set; }
            public string BirthDateText { get; set; }
            public string Gender { get; set; }
            public string GroupName { get; set; }
        }

        public ChildrenWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            cbGender.Items.Add("М");
            cbGender.Items.Add("Ж");

            LoadGroups();
            LoadChildren();
        }

        private void LoadGroups()
        {
            cbGroups.ItemsSource = App.context.Groups.ToList();
            cbGroups.DisplayMemberPath = "Name";
            cbGroups.SelectedValuePath = "Id";
        }

        private void LoadChildren()
        {
            var list = App.context.Children
                .ToList()
                .Select(c => new ChildView
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    BirthDate = c.BirthDate,
                    BirthDateText = c.BirthDate.ToString("dd.MM.yyyy"),
                    Gender = c.Gender,
                    GroupName = c.Groups != null ? c.Groups.Name : "—"
                })
                .ToList();

            dgChildren.ItemsSource = list;
        }

        private ChildView GetSelectedChild()
        {
            return dgChildren.SelectedItem as ChildView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tbFullName.Text))
                {
                    MessageBox.Show("Введите ФИО ребенка.");
                    return;
                }

                if (dpBirthDate.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату рождения.");
                    return;
                }

                if (cbGender.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пол.");
                    return;
                }

                Children child = new Children()
                {
                    FullName = tbFullName.Text.Trim(),
                    BirthDate = dpBirthDate.SelectedDate.Value,
                    Gender = cbGender.SelectedItem.ToString(),
                    GroupId = cbGroups.SelectedValue != null ? (int?)cbGroups.SelectedValue : null
                };

                App.context.Children.Add(child);
                App.context.SaveChanges();

                MessageBox.Show("Ребенок добавлен.");
                LoadChildren();
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
                ChildView selected = GetSelectedChild();
                if (selected == null)
                {
                    MessageBox.Show("Выберите запись.");
                    return;
                }

                var child = App.context.Children.FirstOrDefault(c => c.Id == selected.Id);
                if (child == null)
                {
                    MessageBox.Show("Ребенок не найден.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(tbFullName.Text))
                {
                    MessageBox.Show("Введите ФИО ребенка.");
                    return;
                }

                if (dpBirthDate.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату рождения.");
                    return;
                }

                if (cbGender.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пол.");
                    return;
                }

                child.FullName = tbFullName.Text.Trim();
                child.BirthDate = dpBirthDate.SelectedDate.Value;
                child.Gender = cbGender.SelectedItem.ToString();
                child.GroupId = cbGroups.SelectedValue != null ? (int?)cbGroups.SelectedValue : null;

                App.context.SaveChanges();
                MessageBox.Show("Данные обновлены.");
                LoadChildren();
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
                ChildView selected = GetSelectedChild();
                if (selected == null)
                {
                    MessageBox.Show("Выберите запись.");
                    return;
                }

                var child = App.context.Children.FirstOrDefault(c => c.Id == selected.Id);
                if (child == null)
                {
                    MessageBox.Show("Ребенок не найден.");
                    return;
                }

                var result = MessageBox.Show("Удалить выбранного ребенка?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                App.context.Children.Remove(child);
                App.context.SaveChanges();

                MessageBox.Show("Ребенок удален.");
                LoadChildren();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadChildren();
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
            dpBirthDate.SelectedDate = null;
            cbGender.SelectedItem = null;
            cbGroups.SelectedItem = null;
        }
    }
}