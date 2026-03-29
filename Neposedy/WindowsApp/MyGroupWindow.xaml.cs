using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Neposedy.WindowsApp
{
    public partial class MyGroupWindow : Window
    {
        public class ChildView
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public DateTime BirthDate { get; set; }
            public string Gender { get; set; }

            public string BirthDateText
            {
                get { return "Дата рождения: " + BirthDate.ToString("dd.MM.yyyy"); }
            }

            public string GenderText
            {
                get { return "Пол: " + Gender; }
            }
        }

        public MyGroupWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            LoadGroupInfo();
        }

        private void LoadGroupInfo()
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

                var teacher = App.context.Teachers.FirstOrDefault(t => t.UserId == LoginWindow.CurrentUser.Id);

                if (teacher == null)
                {
                    MessageBox.Show("Воспитатель не найден.");
                    return;
                }

                var group = App.context.Groups.FirstOrDefault(g => g.TeacherId == teacher.Id);

                if (group == null)
                {
                    tbGroupName.Text = "Группа не назначена";
                    tbAgeRange.Text = "—";
                    tbDescription.Text = "Для текущего воспитателя группа пока не назначена.";
                    lbChildren.ItemsSource = null;
                    tbChildrenCount.Text = "Количество детей: 0";
                    return;
                }

                tbGroupName.Text = group.Name;
                tbAgeRange.Text = string.IsNullOrWhiteSpace(group.AgeRange) ? "—" : group.AgeRange;
                tbDescription.Text = string.IsNullOrWhiteSpace(group.Description) ? "Описание отсутствует." : group.Description;

                var children = App.context.Children
                    .Where(c => c.GroupId == group.Id)
                    .ToList()
                    .Select(c => new ChildView
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        BirthDate = c.BirthDate,
                        Gender = c.Gender
                    })
                    .ToList();

                lbChildren.ItemsSource = children;
                tbChildrenCount.Text = "Количество детей: " + children.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки группы: " + ex.Message);
            }
        }

        private void lbChildren_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
            LoadGroupInfo();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            TeacherWindow teacherWindow = new TeacherWindow();
            teacherWindow.Show();
            Close();
        }
    }
}