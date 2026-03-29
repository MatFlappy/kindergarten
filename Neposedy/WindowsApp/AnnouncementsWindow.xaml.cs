using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;

namespace Neposedy.WindowsApp
{
    public partial class AnnouncementsWindow : Window
    {
        private Groups currentGroup;
        private Teachers currentTeacher;

        public class AnnouncementView
        {
            public int Id { get; set; }
            public DateTime PublishDate { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }

            public string PublishDateText
            {
                get { return PublishDate.ToString("dd.MM.yyyy HH:mm"); }
            }
        }

        public AnnouncementsWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            LoadGroupData();
        }

        private void LoadGroupData()
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

                currentTeacher = App.context.Teachers.FirstOrDefault(t => t.UserId == LoginWindow.CurrentUser.Id);

                if (currentTeacher == null)
                {
                    MessageBox.Show("Воспитатель не найден.");
                    return;
                }

                currentGroup = App.context.Groups.FirstOrDefault(g => g.TeacherId == currentTeacher.Id);

                if (currentGroup == null)
                {
                    tbGroupInfo.Text = "Группа не назначена";
                    dgAnnouncements.ItemsSource = null;
                    return;
                }

                tbGroupInfo.Text = "Группа: " + currentGroup.Name;
                LoadAnnouncements();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void LoadAnnouncements()
        {
            try
            {
                if (currentGroup == null)
                {
                    dgAnnouncements.ItemsSource = null;
                    return;
                }

                var list = App.context.Announcements
                    .Where(a => a.GroupId == currentGroup.Id)
                    .ToList()
                    .Select(a => new AnnouncementView
                    {
                        Id = a.Id,
                        PublishDate = a.PublishDate,
                        Title = a.Title,
                        Description = a.Description
                    })
                    .OrderByDescending(a => a.PublishDate)
                    .ToList();

                dgAnnouncements.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки объявлений: " + ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentGroup == null || currentTeacher == null)
                {
                    MessageBox.Show("Группа или воспитатель не найдены.");
                    return;
                }

                string title = tbTitle.Text.Trim();
                string description = tbDescription.Text.Trim();

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Заполните заголовок и текст объявления.");
                    return;
                }

                Announcements newAnnouncement = new Announcements()
                {
                    GroupId = currentGroup.Id,
                    TeacherId = currentTeacher.Id,
                    Title = title,
                    Description = description,
                    PublishDate = DateTime.Now
                };

                App.context.Announcements.Add(newAnnouncement);
                App.context.SaveChanges();

                MessageBox.Show("Объявление опубликовано.");
                ClearFields();
                LoadAnnouncements();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            tbTitle.Text = "";
            tbDescription.Text = "";
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAnnouncements();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            TeacherWindow teacherWindow = new TeacherWindow();
            teacherWindow.Show();
            Close();
        }
    }
}