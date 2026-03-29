using Neposedy.Model;
using NeposedyApp.Windows;
using System;
using System.Linq;
using System.Windows;

namespace Neposedy.WindowsApp
{
    public partial class NotesWindow : Window
    {
        private Groups currentGroup;
        private Teachers currentTeacher;

        public class NoteView
        {
            public int Id { get; set; }
            public string ChildName { get; set; }
            public DateTime NoteDate { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }

            public string NoteDateText
            {
                get { return NoteDate.ToString("dd.MM.yyyy HH:mm"); }
            }

            public string TitleText
            {
                get { return string.IsNullOrWhiteSpace(Title) ? "Без заголовка" : Title; }
            }
        }

        public NotesWindow()
        {
            InitializeComponent();

            if (LoginWindow.CurrentUser != null)
            {
                tbWelcome.Text = "Здравствуйте, " + LoginWindow.CurrentUser.FullName;
            }

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

                currentTeacher = App.context.Teachers.FirstOrDefault(t => t.UserId == LoginWindow.CurrentUser.Id);

                if (currentTeacher == null)
                {
                    MessageBox.Show("Воспитатель не найден.");
                    return;
                }

                currentGroup = App.context.Groups.FirstOrDefault(g => g.TeacherId == currentTeacher.Id);

                if (currentGroup == null)
                {
                    tbGroupInfo.Text = "Группа не назначена";
                    cbChildren.ItemsSource = null;
                    dgNotes.ItemsSource = null;
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

                LoadNotes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void LoadNotes()
        {
            try
            {
                if (currentGroup == null)
                {
                    dgNotes.ItemsSource = null;
                    return;
                }

                var notes = App.context.Notes
                    .Where(n => n.Children.GroupId == currentGroup.Id)
                    .ToList()
                    .Select(n => new NoteView
                    {
                        Id = n.Id,
                        ChildName = n.Children != null ? n.Children.FullName : "",
                        NoteDate = n.NoteDate,
                        Title = n.Title,
                        Description = n.Description
                    })
                    .OrderByDescending(n => n.NoteDate)
                    .ToList();

                dgNotes.ItemsSource = notes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заметок: " + ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentGroup == null || currentTeacher == null)
                {
                    MessageBox.Show("Группа или воспитатель не найдены.");
                    return;
                }

                if (cbChildren.SelectedValue == null)
                {
                    MessageBox.Show("Выберите ребенка.");
                    return;
                }

                int childId = Convert.ToInt32(cbChildren.SelectedValue);

                var child = App.context.Children.FirstOrDefault(c => c.Id == childId && c.GroupId == currentGroup.Id);

                if (child == null)
                {
                    MessageBox.Show("Ребенок не найден в вашей группе.");
                    return;
                }

                string title = tbTitle.Text.Trim();
                string description = tbDescription.Text.Trim();

                if (string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Введите описание заметки.");
                    return;
                }

                Notes newNote = new Notes()
                {
                    ChildId = childId,
                    TeacherId = currentTeacher.Id,
                    NoteDate = DateTime.Now,
                    Title = title,
                    Description = description
                };

                App.context.Notes.Add(newNote);
                App.context.SaveChanges();

                MessageBox.Show("Заметка сохранена.");
                ClearFields();
                LoadNotes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            tbTitle.Text = "";
            tbDescription.Text = "";
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadNotes();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            TeacherWindow teacherWindow = new TeacherWindow();
            teacherWindow.Show();
            Close();
        }
    }
}