using Neposedy.WindowsApp;
using System.Windows;

namespace NeposedyApp.Windows
{
    public partial class TeacherWindow : Window
    {
        public TeacherWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }
        }

        private void MyGroup_Click(object sender, RoutedEventArgs e)
        {
            MyGroupWindow myGroupWindow = new MyGroupWindow();
            myGroupWindow.Show();
            Close();
        }

        private void Attendance_Click(object sender, RoutedEventArgs e)
        {
            AttendanceWindow attendanceWindow = new AttendanceWindow();
            attendanceWindow.Show();
            Close();
        }

        private void Notes_Click(object sender, RoutedEventArgs e)
        {
            NotesWindow notesWindow = new NotesWindow();
            notesWindow.Show();
            Close();
        }

        private void Announcements_Click(object sender, RoutedEventArgs e)
        {
            AnnouncementsWindow announcementsWindow = new AnnouncementsWindow();
            announcementsWindow.Show();
            Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow.CurrentUser = null;
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}