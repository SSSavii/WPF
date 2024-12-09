using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Добавьте этот using
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinForms = System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<ModuleItem> modules;
        private ObservableCollection<MentorAssignment> mentorAssignments;

        public MainWindow()
        {
            InitializeComponent();
            InitializeModulesList();
            InitializeConstructor();
            InitializeAnalysisTab();

            // Инициализация тестовых данных для конструктора
            EmployeeComboBox.ItemsSource = new[] { "Иванов И.И.", "Петров П.П.", "Сидоров С.С." };
            DepartmentComboBox.ItemsSource = new[] { "IT отдел", "HR отдел", "Бухгалтерия" };
            mentorAssignments = new ObservableCollection<MentorAssignment>();
            MentorsListView.ItemsSource = mentorAssignments;
        }

        private void InitializeModulesList()
        {
            modules = new ObservableCollection<ModuleItem>();
            ModulesList.ItemsSource = modules;
        }

        private void InitializeConstructor()
        {
            mentorAssignments = new ObservableCollection<MentorAssignment>();
            MentorsListView.ItemsSource = mentorAssignments;
        }

        private void CreateModule_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ModuleDialog();
            if (dialog.ShowDialog() == true)
            {
                modules.Add(new ModuleItem
                {
                    Name = dialog.ModuleName,
                    Position = dialog.Position,
                    Status = "Новый",
                    Developers = dialog.Developers.ToList(),
                    Approvers = dialog.Approvers.ToList(),
                    MainApprover = dialog.MainApprover,
                    Deadline = dialog.Deadline ?? DateTime.Now
                });
            }
        }

        private void EmployeeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateModulesDisplay();
        }

        private void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartmentComboBox.SelectedItem != null)
            {
                switch (DepartmentComboBox.SelectedItem.ToString())
                {
                    case "IT отдел":
                        PositionComboBox.ItemsSource = new[] { "Программист", "Системный администратор", "Тестировщик" };
                        break;
                    case "HR отдел":
                        PositionComboBox.ItemsSource = new[] { "HR менеджер", "Рекрутер", "HR аналитик" };
                        break;
                    case "Бухгалтерия":
                        PositionComboBox.ItemsSource = new[] { "Бухгалтер", "Главный бухгалтер", "Аудитор" };
                        break;
                }
                PositionComboBox.SelectedIndex = -1;
            }
        }

        private void PositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateModulesDisplay();
        }

        private void UpdateModulesDisplay()
        {
            ModulesPanel.Children.Clear();
            mentorAssignments.Clear();

            if (PositionComboBox.SelectedItem == null) return;

            var modules = GetModulesForPosition(PositionComboBox.SelectedItem.ToString());
            foreach (var module in modules)
            {
                ModulesPanel.Children.Add(CreateModulePanel(module));
            }
        }

        private List<string> GetModulesForPosition(string position)
        {
            var modules = new List<string>();
            switch (position)
            {
                case "Программист":
                    modules.AddRange(new[] {
                        "Введение в разработку",
                        "Стандарты кодирования",
                        "Работа с системой контроля версий",
                        "Процессы разработки"
                    });
                    break;
                case "HR менеджер":
                    modules.AddRange(new[] {
                        "Основы HR",
                        "Процессы найма",
                        "Адаптация персонала",
                        "Развитие персонала"
                    });
                    break;
                case "Системный администратор":
                    modules.AddRange(new[] {
                        "Сетевая инфраструктура",
                        "Безопасность систем",
                        "Администрирование серверов",
                        "Поддержка пользователей"
                    });
                    break;
                default:
                    modules.AddRange(new[] {
                        $"Модуль 1 для {position}",
                        $"Модуль 2 для {position}",
                        $"Модуль 3 для {position}"
                    });
                    break;
            }
            return modules;
        }

        private Border CreateModulePanel(string moduleName)
        {
            var border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                Background = Brushes.White,
                MinWidth = 200,
                MinHeight = 100
            };

            var panel = new StackPanel();

            var checkBox = new CheckBox
            {
                Content = moduleName,
                Margin = new Thickness(0, 0, 0, 5)
            };
            checkBox.Checked += ModuleCheckBox_Checked;
            checkBox.Unchecked += ModuleCheckBox_Unchecked;

            panel.Children.Add(checkBox);

            var tooltip = new ToolTip
            {
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = moduleName,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 0, 0, 5)
                        },
                        new TextBlock
                        {
                            Text = $"Подробное описание модуля '{moduleName}'\n" +
                                   $"Длительность: 2 недели\n" +
                                   $"Необходимые материалы: ...\n" +
                                   $"Ожидаемые результаты: ...",
                            TextWrapping = TextWrapping.Wrap,
                            MaxWidth = 300
                        }
                    }
                }
            };
            border.ToolTip = tooltip;

            border.Child = panel;
            return border;
        }


        private void ModuleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var moduleName = checkBox.Content.ToString();
                mentorAssignments.Add(new MentorAssignment
                {
                    ModuleName = moduleName,
                    AvailableMentors = new[] {
                        "Иванов И.И.",
                        "Петров П.П.",
                        "Сидоров С.С."
                    }
                });
            }
        }

        private void ModuleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var moduleName = checkBox.Content.ToString();
                var assignment = mentorAssignments.FirstOrDefault(m => m.ModuleName == moduleName);
                if (assignment != null)
                {
                    mentorAssignments.Remove(assignment);
                }
            }
        }

        private void GenerateProgram_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateConstructorForm()) return;

            var employeeName = EmployeeComboBox.SelectedItem.ToString();
            var department = DepartmentComboBox.SelectedItem.ToString();
            var position = PositionComboBox.SelectedItem.ToString();

            MessageBox.Show(
                $"Программа адаптации сформирована и сохранена:\n\n" +
                $"Файл: {department}_{position}_{employeeName}_{DateTime.Now:dd_MM_yyyy}.xlsx\n" +
                $"Путь: Documents\\Адаптационные программы\n\n" +
                $"Уведомления отправлены на почту наставникам и сотруднику.",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private bool ValidateConstructorForm()
        {
            if (EmployeeComboBox.SelectedItem == null ||
                DepartmentComboBox.SelectedItem == null ||
                PositionComboBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Пожалуйста, заполните все обязательные поля",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            if (!mentorAssignments.Any())
            {
                MessageBox.Show(
                    "Выберите хотя бы один модуль",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            if (mentorAssignments.Any(m => m.SelectedMentor == null))
            {
                MessageBox.Show(
                    "Назначьте наставников для всех выбранных модулей",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }

            return true;
        }

        private void InitializeAnalysisTab()
        {
            // Заполнение фильтров
            AnalysisDepartmentFilter.ItemsSource = new[] { "IT отдел", "HR отдел", "Бухгалтерия" };
            AnalysisPositionFilter.ItemsSource = new[] {
                "Программист", "Системный администратор", "Тестировщик",
                "HR менеджер", "Рекрутер", "HR аналитик",
                "Бухгалтер", "Главный бухгалтер", "Аудитор"
            };

            // Генерация тестовых данных
            var analysisData = GenerateTestAnalysisData();
            AdaptationReportsGrid.ItemsSource = analysisData;

            // Настройка диаграмм
            SetupErrorChart(analysisData);
            SetupCompletionChart(analysisData);
        }
        /*Вот сюда вставлять новые данные*/
        private List<AnalysisData> GenerateTestAnalysisData()
        {
            return new List<AnalysisData>
            {
                new AnalysisData
                {
                    EmployeeName = "Иванов И.И.",
                    Department = "IT отдел",
                    Position = "Программист",
                    CompletionPercentage = 85.5,
                    ErrorCount = 3,
                    Status = "Успешно завершена"
                },
                new AnalysisData
                {
                    EmployeeName = "Петров П.П.",
                    Department = "HR отдел",
                    Position = "HR менеджер",
                    CompletionPercentage = 92.0,
                    ErrorCount = 1,
                    Status = "Успешно завершена"
                },
                new AnalysisData
                {
                    EmployeeName = "Сидоров С.С.",
                    Department = "Бухгалтерия",
                    Position = "Бухгалтер",
                    CompletionPercentage = 75.3,
                    ErrorCount = 5,
                    Status = "В процессе"
                },
                new AnalysisData
                {
                    EmployeeName = "Смирнов А.А.",
                    Department = "IT отдел",
                    Position = "Системный администратор",
                    CompletionPercentage = 65.7,
                    ErrorCount = 4,
                    Status = "В процессе"
                },
                new AnalysisData
                {
                    EmployeeName = "Кузнецова М.В.",
                    Department = "HR отдел",
                    Position = "Рекрутер",
                    CompletionPercentage = 95.2,
                    ErrorCount = 0,
                    Status = "Успешно завершена"
                }
            };
        }

        private void SetupErrorChart(List<AnalysisData> data)
        {
            Chart errorChart = new Chart();
            errorChart.ChartAreas.Add(new ChartArea());

            Series errorSeries = new Series("Ошибки")
            {
                ChartType = SeriesChartType.Column
            };

            foreach (var item in data)
            {
                errorSeries.Points.AddXY(item.EmployeeName, item.ErrorCount);
            }

            errorChart.Series.Add(errorSeries);
            errorChart.Titles.Add(new Title("Количество ошибок по сотрудникам"));

            ErrorChartHost.Child = errorChart;
        }

        private void SetupCompletionChart(List<AnalysisData> data)
        {
            Chart completionChart = new Chart();
            completionChart.ChartAreas.Add(new ChartArea());

            Series completionSeries = new Series("Выполнение")
            {
                ChartType = SeriesChartType.Pie
            };

            var completionGroups = data.GroupBy(d =>
                d.CompletionPercentage < 50 ? "Низкий" :
                d.CompletionPercentage < 80 ? "Средний" : "Высокий"
            );

            foreach (var group in completionGroups)
            {
                completionSeries.Points.AddXY(
                    group.Key,
                    group.Count()
                );
            }

            completionChart.Series.Add(completionSeries);
            completionChart.Titles.Add(new Title("Уровень выполнения программ адаптации"));

            CompletionChartHost.Child = completionChart;
        }

        private void ApplyAnalysisFilter_Click(object sender, RoutedEventArgs e)
        {
            var analysisData = GenerateTestAnalysisData();

            // Фильтрация по отделу
            if (AnalysisDepartmentFilter.SelectedItem != null)
            {
                analysisData = analysisData
                    .Where(d => d.Department == AnalysisDepartmentFilter.SelectedItem.ToString())
                    .ToList();
            }

            // Фильтрация по должности
            if (AnalysisPositionFilter.SelectedItem != null)
            {
                analysisData = analysisData
                    .Where(d => d.Position == AnalysisPositionFilter.SelectedItem.ToString())
                    .ToList();
            }

            // Фильтрация по кварталу (здесь можно добавить логику, если у вас будет дата)
            // Например, если добавить DateTime в AnalysisData

            AdaptationReportsGrid.ItemsSource = analysisData;
            SetupErrorChart(analysisData);
            SetupCompletionChart(analysisData);
        }
    }

    public class ModuleItem
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public List<string> Developers { get; set; }
        public List<string> Approvers { get; set; }
        public string MainApprover { get; set; }
        public DateTime Deadline { get; set; }
    }

    public class MentorAssignment
    {
        public string ModuleName { get; set; }
        public string[] AvailableMentors { get; set; }
        public string SelectedMentor { get; set; }
    }

    public class AnalysisData
    {
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public double CompletionPercentage { get; set; }
        public int ErrorCount { get; set; }
        public string Status { get; set; }
    }
}