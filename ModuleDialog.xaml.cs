using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class ModuleDialog : Window
    {
        public string ModuleName => NameTextBox.Text;
        public string Position => PositionTextBox.Text;
        public ObservableCollection<string> Developers { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Approvers { get; } = new ObservableCollection<string>();
        public string MainApprover => MainApproverComboBox.SelectedItem?.ToString();
        public DateTime? Deadline => DeadlinePicker.SelectedDate;

        public ModuleDialog()
        {
            InitializeComponent();
            DevelopersListBox.ItemsSource = Developers;
            ApproversListBox.ItemsSource = Approvers;
            DeadlinePicker.SelectedDate = DateTime.Now.AddDays(30);
        }

        private void AddDeveloper_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(DeveloperTextBox.Text))
            {
                Developers.Add(DeveloperTextBox.Text);
                DeveloperTextBox.Clear();
            }
        }

        private void RemoveDeveloper_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is string developer)
            {
                Developers.Remove(developer);
            }
        }

        private void AddApprover_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ApproverTextBox.Text))
            {
                Approvers.Add(ApproverTextBox.Text);
                ApproverTextBox.Clear();
                // Обновляем список доступных главных согласующих
                MainApproverComboBox.ItemsSource = Approvers;
            }
        }

        private void RemoveApprover_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is string approver)
            {
                Approvers.Remove(approver);
                // Обновляем список доступных главных согласующих
                MainApproverComboBox.ItemsSource = Approvers;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModuleName) ||
                string.IsNullOrWhiteSpace(Position) ||
                !Developers.Any() ||
                !Approvers.Any() ||
                MainApprover == null ||
                Deadline == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}