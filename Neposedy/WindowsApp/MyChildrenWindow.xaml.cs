using Neposedy;
using Neposedy.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NeposedyApp.Windows
{
    public partial class MyChildrenWindow : Window
    {
        public class ChildView
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public DateTime? BirthDate { get; set; }
            public string Gender { get; set; }
            public string GroupName { get; set; }
            public string TeacherName { get; set; }

            public string BirthDateText
            {
                get
                {
                    return "Дата рождения: " + (BirthDate.HasValue ? BirthDate.Value.ToString("dd.MM.yyyy") : "");
                }
            }

            public string GenderText
            {
                get
                {
                    return "Пол: " + Gender;
                }
            }

            public string GroupNameText
            {
                get
                {
                    return "Группа: " + GroupName;
                }
            }

            public string TeacherNameText
            {
                get
                {
                    return "Воспитатель: " + TeacherName;
                }
            }
        }

        public MyChildrenWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            LoadChildren();
        }

        private void LoadChildren()
        {
            try
            {
                if (LoginWindow.CurrentUser == null)
                {
                    MessageBox.Show("Сначала выполните вход.");
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    Close();
                    return;
                }

                var parent = App.context.Parents.FirstOrDefault(p => p.UserId == LoginWindow.CurrentUser.Id);

                if (parent == null)
                {
                    MessageBox.Show("Родитель не найден.");
                    return;
                }

                var children = App.context.ChildParents
                    .Where(cp => cp.ParentId == parent.Id)
                    .ToList()
                    .Select(cp => cp.Children)
                    .Where(c => c != null)
                    .Select(c => new ChildView
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        BirthDate = c.BirthDate,
                        Gender = c.Gender,
                        GroupName = c.Groups != null ? c.Groups.Name : "Не назначена",
                        TeacherName = c.Groups != null && c.Groups.Teachers != null && c.Groups.Teachers.Users != null
                            ? c.Groups.Teachers.Users.FullName
                            : "Не назначен"
                    })
                    .ToList();

                lbChildren.ItemsSource = children;
                tbCount.Text = "Количество детей: " + children.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void lbChildren_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChildView selectedChild = lbChildren.SelectedItem as ChildView;

            if (selectedChild == null)
                return;

            var child = App.context.Children.FirstOrDefault(c => c.Id == selectedChild.Id);

            if (child == null)
            {
                MessageBox.Show("Ребенок не найден.");
                return;
            }

            ChildDetailsWindow childDetailsWindow = new ChildDetailsWindow(child);
            childDetailsWindow.ShowDialog();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadChildren();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow parentWindow = new ParentWindow();
            parentWindow.Show();
            Close();
        }
    }
}