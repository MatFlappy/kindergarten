using Neposedy;
using Neposedy.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NeposedyApp.Windows
{
    public partial class ParentNotesWindow : Window
    {
        public class NoteView
        {
            public int Id { get; set; }
            public DateTime? NoteDate { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string TeacherName { get; set; }

            public string NoteDateText
            {
                get
                {
                    return NoteDate.HasValue ? NoteDate.Value.ToString("dd.MM.yyyy HH:mm") : "";
                }
            }

            public string TitleText
            {
                get
                {
                    return string.IsNullOrWhiteSpace(Title) ? "Без заголовка" : Title;
                }
            }
        }

        public ParentNotesWindow()
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
                    tbCount.Text = "Заметок: 0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки детей: " + ex.Message);
            }
        }

        private void LoadNotes()
        {
            try
            {
                if (cbChildren.SelectedValue == null)
                {
                    dgNotes.ItemsSource = null;
                    tbCount.Text = "Заметок: 0";
                    return;
                }

                int childId = Convert.ToInt32(cbChildren.SelectedValue);

                var notesList = App.context.Notes
                    .Where(n => n.ChildId == childId)
                    .ToList()
                    .Select(n => new NoteView
                    {
                        Id = n.Id,
                        NoteDate = n.NoteDate,
                        Title = n.Title,
                        Description = n.Description,
                        TeacherName = n.Teachers != null && n.Teachers.Users != null
                            ? n.Teachers.Users.FullName
                            : "Не указан"
                    })
                    .OrderByDescending(n => n.NoteDate)
                    .ToList();

                dgNotes.ItemsSource = notesList;
                tbCount.Text = "Заметок: " + notesList.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заметок: " + ex.Message);
            }
        }

        private void cbChildren_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadNotes();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadNotes();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow parentWindow = new ParentWindow();
            parentWindow.Show();
            Close();
        }
    }
}