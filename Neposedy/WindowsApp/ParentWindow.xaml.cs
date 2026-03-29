using System.Windows;

namespace NeposedyApp.Windows
{
    public partial class ParentWindow : Window
    {
        public ParentWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }
        }

        private void MyChildren_Click(object sender, RoutedEventArgs e)
        {
            MyChildrenWindow myChildrenWindow = new MyChildrenWindow();
            myChildrenWindow.Show();
            Close();
        }

        private void Attendance_Click(object sender, RoutedEventArgs e)
        {
            ParentAttendanceWindow parentAttendanceWindow = new ParentAttendanceWindow();
            parentAttendanceWindow.Show();
            Close();
        }

        private void Notes_Click(object sender, RoutedEventArgs e)
        {
            ParentNotesWindow parentNotesWindow = new ParentNotesWindow();
            parentNotesWindow.Show();
            Close();
        }

        private void Announcements_Click(object sender, RoutedEventArgs e)
        {
            ParentAnnouncementsWindow parentAnnouncementsWindow = new ParentAnnouncementsWindow();
            parentAnnouncementsWindow.Show();
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