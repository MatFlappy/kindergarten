using Neposedy.WindowsApp;
using System.Windows;

namespace NeposedyApp.Windows
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }
        }

        private void Children_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно управления детьми сделаем следующим.");
        }

        private void Parents_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно управления родителями сделаем следующим.");
        }

        private void Teachers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно управления воспитателями сделаем следующим.");
        }

        private void Groups_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно управления группами сделаем следующим.");
        }

        private void Applications_Click(object sender, RoutedEventArgs e)
        {
            ApplicationsWindow applicationsWindow = new ApplicationsWindow();
            applicationsWindow.Show();
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