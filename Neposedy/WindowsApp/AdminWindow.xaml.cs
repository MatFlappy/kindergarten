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
            ChildrenWindow window = new ChildrenWindow();
            window.Show();
            Close();
        }

        private void Parents_Click(object sender, RoutedEventArgs e)
        {
            ParentsWindow window = new ParentsWindow();
            window.Show();
            Close();
        }

        private void Teachers_Click(object sender, RoutedEventArgs e)
        {
            TeachersWindow window = new TeachersWindow();
            window.Show();
            Close();
        }

        private void Groups_Click(object sender, RoutedEventArgs e)
        {
            GroupsWindow window = new GroupsWindow();
            window.Show();
            Close();
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