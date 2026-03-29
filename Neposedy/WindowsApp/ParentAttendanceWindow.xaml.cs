using Neposedy;
using Neposedy.Model;    
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NeposedyApp.Windows
{
    public partial class ParentAttendanceWindow : Window
    {
        public class AttendanceView
        {
            public int Id { get; set; }
            public DateTime? Date { get; set; }
            public string Status { get; set; }
            public string Comment { get; set; }

            public string DateText
            {
                get
                {
                    return Date.HasValue ? Date.Value.ToString("dd.MM.yyyy") : "";
                }
            }

            public string CommentText
            {
                get
                {
                    return string.IsNullOrWhiteSpace(Comment) ? "—" : Comment;
                }
            }
        }

        public ParentAttendanceWindow()
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
                    tbCount.Text = "Записей: 0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки детей: " + ex.Message);
            }
        }

        private void LoadAttendance()
        {
            try
            {
                if (cbChildren.SelectedValue == null)
                {
                    dgAttendance.ItemsSource = null;
                    tbCount.Text = "Записей: 0";
                    return;
                }

                int childId = Convert.ToInt32(cbChildren.SelectedValue);

                var attendanceList = App.context.Attendance
                    .Where(a => a.ChildId == childId)
                    .ToList()
                    .Select(a => new AttendanceView
                    {
                        Id = a.Id,
                        Date = a.Date,
                        Status = a.Status,
                        Comment = a.Comment
                    })
                    .OrderByDescending(a => a.Date)
                    .ToList();

                dgAttendance.ItemsSource = attendanceList;
                tbCount.Text = "Записей: " + attendanceList.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки посещаемости: " + ex.Message);
            }
        }

        private void cbChildren_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAttendance();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAttendance();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow parentWindow = new ParentWindow();
            parentWindow.Show();
            Close();
        }
    }
}