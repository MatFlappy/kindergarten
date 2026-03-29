using Neposedy;
using Neposedy.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NeposedyApp.Windows
{
    public partial class ParentAnnouncementsWindow : Window
    {
        public class AnnouncementView
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? PublishDate { get; set; }
            public string TeacherName { get; set; }

            public string PublishDateText
            {
                get
                {
                    return "Дата публикации: " + (PublishDate.HasValue ? PublishDate.Value.ToString("dd.MM.yyyy HH:mm") : "");
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

        public ParentAnnouncementsWindow()
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
                    .Select(c => new
                    {
                        c.Id,
                        c.FullName
                    })
                    .ToList();

                cbChildren.ItemsSource = children;
                cbChildren.DisplayMemberPath = "FullName";
                cbChildren.SelectedValuePath = "Id";

                if (children.Count > 0)
                    cbChildren.SelectedIndex = 0;
                else
                    tbCount.Text = "Объявлений: 0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки детей: " + ex.Message);
            }
        }

        private void LoadAnnouncements()
        {
            try
            {
                if (cbChildren.SelectedValue == null)
                {
                    lbAnnouncements.ItemsSource = null;
                    tbCount.Text = "Объявлений: 0";
                    return;
                }

                int childId = Convert.ToInt32(cbChildren.SelectedValue);

                var child = App.context.Children.FirstOrDefault(c => c.Id == childId);

                if (child == null || child.GroupId == null)
                {
                    lbAnnouncements.ItemsSource = null;
                    tbCount.Text = "Объявлений: 0";
                    return;
                }

                int groupId = child.GroupId.Value;

                var announcements = App.context.Announcements
                    .Where(a => a.GroupId == groupId)
                    .ToList()
                    .Select(a => new AnnouncementView
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        PublishDate = a.PublishDate,
                        TeacherName = a.Teachers != null && a.Teachers.Users != null
                            ? a.Teachers.Users.FullName
                            : "Не указан"
                    })
                    .OrderByDescending(a => a.PublishDate)
                    .ToList();

                lbAnnouncements.ItemsSource = announcements;
                tbCount.Text = "Объявлений: " + announcements.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки объявлений: " + ex.Message);
            }
        }

        private void cbChildren_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAnnouncements();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAnnouncements();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow parentWindow = new ParentWindow();
            parentWindow.Show();
            Close();
        }
    }
}