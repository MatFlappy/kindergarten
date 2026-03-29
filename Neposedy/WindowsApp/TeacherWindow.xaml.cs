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
            MessageBox.Show("Окно моей группы сделаем следующим.");
        }

        private void Attendance_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно посещаемости сделаем следующим.");
        }

        private void Notes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно заметок сделаем следующим.");
        }

        private void Announcements_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно объявлений сделаем следующим.");
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