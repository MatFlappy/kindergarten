using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;

namespace Neposedy.WindowsApp
{
    public partial class ApplicationsWindow : Window
    {
        public class ApplicationView
        {
            public int Id { get; set; }
            public string ParentFullName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string ChildFullName { get; set; }
            public DateTime? ChildBirthDate { get; set; }
            public string DesiredGroup { get; set; }
            public string Comment { get; set; }
            public DateTime ApplicationDate { get; set; }
            public string Status { get; set; }

            public string EmailText
            {
                get { return string.IsNullOrWhiteSpace(Email) ? "—" : Email; }
            }

            public string ChildBirthDateText
            {
                get { return ChildBirthDate.HasValue ? ChildBirthDate.Value.ToString("dd.MM.yyyy") : "—"; }
            }

            public string DesiredGroupText
            {
                get { return string.IsNullOrWhiteSpace(DesiredGroup) ? "—" : DesiredGroup; }
            }

            public string CommentText
            {
                get { return string.IsNullOrWhiteSpace(Comment) ? "—" : Comment; }
            }

            public string ApplicationDateText
            {
                get { return ApplicationDate.ToString("dd.MM.yyyy HH:mm"); }
            }
        }

        public ApplicationsWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            cbStatus.Items.Add("Новая");
            cbStatus.Items.Add("В обработке");
            cbStatus.Items.Add("Принята");
            cbStatus.Items.Add("Отклонена");
            cbStatus.SelectedIndex = 0;

            LoadApplications();
        }

        private void LoadApplications()
        {
            try
            {
                var list = App.context.Applications
                    .ToList()
                    .Select(a => new ApplicationView
                    {
                        Id = a.Id,
                        ParentFullName = a.ParentFullName,
                        Phone = a.Phone,
                        Email = a.Email,
                        ChildFullName = a.ChildFullName,
                        ChildBirthDate = a.ChildBirthDate,
                        DesiredGroup = a.DesiredGroup,
                        Comment = a.Comment,
                        ApplicationDate = a.ApplicationDate,
                        Status = a.Status
                    })
                    .OrderByDescending(a => a.ApplicationDate)
                    .ToList();

                dgApplications.ItemsSource = list;
                tbCount.Text = "Количество заявок: " + list.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заявок: " + ex.Message);
            }
        }

        private ApplicationView GetSelectedApplication()
        {
            return dgApplications.SelectedItem as ApplicationView;
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedApplication = GetSelectedApplication();

                if (selectedApplication == null)
                {
                    MessageBox.Show("Выберите заявку.");
                    return;
                }

                if (cbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус.");
                    return;
                }

                string newStatus = cbStatus.SelectedItem.ToString();

                var application = App.context.Applications.FirstOrDefault(a => a.Id == selectedApplication.Id);

                if (application == null)
                {
                    MessageBox.Show("Заявка не найдена.");
                    return;
                }

                application.Status = newStatus;
                App.context.SaveChanges();

                MessageBox.Show("Статус заявки изменен.");
                LoadApplications();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения статуса: " + ex.Message);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedApplication = GetSelectedApplication();

                if (selectedApplication == null)
                {
                    MessageBox.Show("Выберите заявку.");
                    return;
                }

                var result = MessageBox.Show("Удалить выбранную заявку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                var application = App.context.Applications.FirstOrDefault(a => a.Id == selectedApplication.Id);

                if (application == null)
                {
                    MessageBox.Show("Заявка не найдена.");
                    return;
                }

                App.context.Applications.Remove(application);
                App.context.SaveChanges();

                MessageBox.Show("Заявка удалена.");
                LoadApplications();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления заявки: " + ex.Message);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadApplications();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
            Close();
        }
    }
}