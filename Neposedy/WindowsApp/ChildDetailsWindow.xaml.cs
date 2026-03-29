using Neposedy.Model;
using System;
using System.Linq;
using System.Windows;

namespace NeposedyApp.Windows
{
    public partial class ChildDetailsWindow : Window
    {
        private Children _child;

        public ChildDetailsWindow(Children child)
        {
            InitializeComponent();
            _child = child;
            LoadChildInfo();
        }

        private void LoadChildInfo()
        {
            if (_child == null)
            {
                MessageBox.Show("Данные о ребенке не найдены.");
                Close();
                return;
            }

            tbFullName.Text = _child.FullName;
            tbBirthDate.Text = "Дата рождения: " + _child.BirthDate.ToString("dd.MM.yyyy");
            tbGender.Text = "Пол: " + _child.Gender;

            if (_child.AdmissionDate.HasValue)
                tbAdmissionDate.Text = "Зачислен: " + _child.AdmissionDate.Value.ToString("dd.MM.yyyy");
            else
                tbAdmissionDate.Text = "Зачислен: не указано";

            tbGroup.Text = "Группа: " + (_child.Groups != null ? _child.Groups.Name : "Не назначена");

            if (_child.Groups != null && _child.Groups.Teachers != null && _child.Groups.Teachers.Users != null)
                tbTeacher.Text = "Воспитатель: " + _child.Groups.Teachers.Users.FullName;
            else
                tbTeacher.Text = "Воспитатель: не назначен";

            tbMedicalInfo.Text = string.IsNullOrWhiteSpace(_child.MedicalInfo)
                ? "Медицинская информация отсутствует."
                : _child.MedicalInfo;

            tbNotes.Text = string.IsNullOrWhiteSpace(_child.Notes)
                ? "Дополнительные заметки отсутствуют."
                : _child.Notes;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MyChildrenWindow myChildrenWindow = new MyChildrenWindow();
            myChildrenWindow.Show();
            Close();
        }
    }
}