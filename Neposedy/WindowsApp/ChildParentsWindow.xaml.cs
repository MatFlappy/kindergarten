using Neposedy.Model;
using NeposedyApp.Windows;
using System.Linq;
using System.Windows;

namespace Neposedy.WindowsApp
{
    public partial class ChildParentsWindow : Window
    {
        public class ChildParentView
        {
            public int Id { get; set; }
            public string ChildName { get; set; }
            public string ParentName { get; set; }
            public string ParentPhone { get; set; }
            public string RelationshipType { get; set; }

            public string RelationshipTypeText
            {
                get { return string.IsNullOrWhiteSpace(RelationshipType) ? "—" : RelationshipType; }
            }
        }

        public ChildParentsWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

            cbRelation.Items.Add("Мать");
            cbRelation.Items.Add("Отец");
            cbRelation.Items.Add("Опекун");
            cbRelation.Items.Add("Бабушка");
            cbRelation.Items.Add("Дедушка");
            cbRelation.SelectedIndex = 0;

            LoadChildren();
            LoadParents();
            LoadChildParents();
        }

        private void LoadChildren()
        {
            var children = App.context.Children
                .ToList()
                .Select(c => new
                {
                    c.Id,
                    c.FullName
                })
                .ToList();

            cbChildren.ItemsSource = children;
            cbChildren.DisplayMemberPath = "FullName";
            cbChildren.SelectedValuePath = "Id";
        }

        private void LoadParents()
        {
            var parents = App.context.Parents
                .ToList()
                .Select(p => new
                {
                    p.Id,
                    FullName = p.Users != null ? p.Users.FullName : "—"
                })
                .ToList();

            cbParents.ItemsSource = parents;
            cbParents.DisplayMemberPath = "FullName";
            cbParents.SelectedValuePath = "Id";
        }

        private void LoadChildParents()
        {
            var list = App.context.ChildParents
                .ToList()
                .Select(cp => new ChildParentView
                {
                    Id = cp.Id,
                    ChildName = cp.Children != null ? cp.Children.FullName : "—",
                    ParentName = cp.Parents != null && cp.Parents.Users != null ? cp.Parents.Users.FullName : "—",
                    ParentPhone = cp.Parents != null ? cp.Parents.Phone : "—",
                    RelationshipType = cp.RelationshipType
                })
                .ToList();

            dgChildParents.ItemsSource = list;
        }

        private ChildParentView GetSelectedLink()
        {
            return dgChildParents.SelectedItem as ChildParentView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbChildren.SelectedValue == null)
                {
                    MessageBox.Show("Выберите ребенка.");
                    return;
                }

                if (cbParents.SelectedValue == null)
                {
                    MessageBox.Show("Выберите родителя.");
                    return;
                }

                int childId = (int)cbChildren.SelectedValue;
                int parentId = (int)cbParents.SelectedValue;
                string relation = cbRelation.SelectedItem != null ? cbRelation.SelectedItem.ToString() : "";

                var existing = App.context.ChildParents.FirstOrDefault(cp => cp.ChildId == childId && cp.ParentId == parentId);

                if (existing != null)
                {
                    MessageBox.Show("Такая связь уже существует.");
                    return;
                }

                ChildParents link = new ChildParents()
                {
                    ChildId = childId,
                    ParentId = parentId,
                    RelationshipType = relation
                };

                App.context.ChildParents.Add(link);
                App.context.SaveChanges();

                MessageBox.Show("Связь добавлена.");
                LoadChildParents();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChildParentView selected = GetSelectedLink();
                if (selected == null)
                {
                    MessageBox.Show("Выберите связь.");
                    return;
                }

                var link = App.context.ChildParents.FirstOrDefault(cp => cp.Id == selected.Id);
                if (link == null)
                {
                    MessageBox.Show("Связь не найдена.");
                    return;
                }

                var result = MessageBox.Show("Удалить выбранную связь?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                App.context.ChildParents.Remove(link);
                App.context.SaveChanges();

                MessageBox.Show("Связь удалена.");
                LoadChildParents();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadChildParents();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
            Close();
        }
    }
}