using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Neposedy.WindowsApp
{
    public partial class AttendanceWindow : Window
    {
        private Groups currentGroup;

        public class AttendanceView
        {
            public int Id { get; set; }
            public string ChildName { get; set; }
            public DateTime Date { get; set; }
            public string Status { get; set; }
            public string Comment { get; set; }

            public string DateText
            {
                get { return Date.ToString("dd.MM.yyyy"); }
            }

            public string CommentText
            {
                get { return string.IsNullOrWhiteSpace(Comment) ? "—" : Comment; }
            }
        }

        public AttendanceWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            dpDate.SelectedDate = DateTime.Today;

            cbStatus.Items.Add("Присутствовал");
            cbStatus.Items.Add("Отсутствовал");
            cbStatus.Items.Add("Болел");
            cbStatus.SelectedIndex = 0;

            LoadGroupAndChildren();
        }

        private void LoadGroupAndChildren()
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

                currentGroup = App.context.Groups.FirstOrDefault(g => g.TeacherId == teacher.Id);

                if (currentGroup == null)
                {
                    tbGroupInfo.Text = "Группа не назначена";
                    cbChildren.ItemsSource = null;
                    dgAttendance.ItemsSource = null;
                    return;
                }

                tbGroupInfo.Text = "Группа: " + currentGroup.Name;

                var children = App.context.Children
                    .Where(c => c.GroupId == currentGroup.Id)
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

                LoadAttendanceList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void LoadAttendanceList()
        {
            try
            {
                if (currentGroup == null || !dpDate.SelectedDate.HasValue)
                {
                    dgAttendance.ItemsSource = null;
                    return;
                }

                DateTime selectedDate = dpDate.SelectedDate.Value.Date;

                var list = App.context.Attendance
                    .Where(a => a.Children.GroupId == currentGroup.Id && a.Date == selectedDate)
                    .ToList()
                    .Select(a => new AttendanceView
                    {
                        Id = a.Id,
                        ChildName = a.Children != null ? a.Children.FullName : "",
                        Date = a.Date,
                        Status = a.Status,
                        Comment = a.Comment
                    })
                    .OrderBy(a => a.ChildName)
                    .ToList();

                dgAttendance.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки посещаемости: " + ex.Message);
            }
        }

        private void LoadExistingAttendanceForChild()
        {
            try
            {
                if (cbChildren.SelectedValue == null || !dpDate.SelectedDate.HasValue)
                    return;

                int childId = Convert.ToInt32(cbChildren.SelectedValue);
                DateTime selectedDate = dpDate.SelectedDate.Value.Date;

                var attendance = App.context.Attendance
                    .FirstOrDefault(a => a.ChildId == childId && a.Date == selectedDate);

                if (attendance != null)
                {
                    cbStatus.SelectedItem = attendance.Status;
                    tbComment.Text = attendance.Comment;
                }
                else
                {
                    cbStatus.SelectedIndex = 0;
                    tbComment.Text = "";
                }
            }
            catch
            {
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentGroup == null)
                {
                    MessageBox.Show("Группа не найдена.");
                    return;
                }

                if (cbChildren.SelectedValue == null)
                {
                    MessageBox.Show("Выберите ребенка.");
                    return;
                }

                if (!dpDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату.");
                    return;
                }

                if (cbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус.");
                    return;
                }

                int childId = Convert.ToInt32(cbChildren.SelectedValue);
                DateTime selectedDate = dpDate.SelectedDate.Value.Date;
                string status = cbStatus.SelectedItem.ToString();
                string comment = tbComment.Text.Trim();

                var child = App.context.Children.FirstOrDefault(c => c.Id == childId && c.GroupId == currentGroup.Id);

                if (child == null)
                {
                    MessageBox.Show("Ребенок не найден в вашей группе.");
                    return;
                }

                var attendance = App.context.Attendance
                    .FirstOrDefault(a => a.ChildId == childId && a.Date == selectedDate);

                if (attendance == null)
                {
                    Attendance newAttendance = new Attendance()
                    {
                        ChildId = childId,
                        Date = selectedDate,
                        Status = status,
                        Comment = comment
                    };

                    App.context.Attendance.Add(newAttendance);
                }
                else
                {
                    attendance.Status = status;
                    attendance.Comment = comment;
                }

                App.context.SaveChanges();
                MessageBox.Show("Посещаемость сохранена.");
                LoadAttendanceList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            cbStatus.SelectedIndex = 0;
            tbComment.Text = "";
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAttendanceList();
            LoadExistingAttendanceForChild();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            TeacherWindow teacherWindow = new TeacherWindow();
            teacherWindow.Show();
            Close();
        }

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAttendanceList();
            LoadExistingAttendanceForChild();
        }

        private void cbChildren_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadExistingAttendanceForChild();
        }
    }
}