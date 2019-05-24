using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using Ciloci.Flee;
using SqliteORM;
using System.Collections.ObjectModel;
using ReliabilityAnalysis.DataBase;
using ReliabilityAnalysis.Scheme;
using ReliabilityAnalysis.Algorithms;


namespace ReliabilityAnalysis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int ProjectNumber
        {
            set;

            get;

        }
        public delegate void ProjectIsSelected(string message);

        public event ProjectIsSelected True;
        public event ProjectIsSelected False;


        ObservableCollection<Project> projects;
        public MainWindow()
        {
            InitializeComponent();

            Tables.StringConnection = @"data source= C:\Users\Александр\source\repos\ReliabilityAnalysis\ReliabilityAnalysis\AppData\data.db";


            projects = new ObservableCollection<Project>();
            if (False != null)
                False("NoProjectSelected");
        }

        private void ShowEriList(object sender, RoutedEventArgs e)
        {
            ERI eri = new ERI(projects[0]);
            eri.ShowDialog();
        }
        private void ClickTextDesignation(object sender, MouseEventArgs e)
        {
            projects[0].SelectElement(((TextBlock)sender).Text);
            var source = projects[0].SelectedElement.ElementOfGrid;
            GridProp.ItemsSource = source;
        }
        private void CreateNewProject(object sender, RoutedEventArgs e)
        {
            ProjectNumber = 1;
            projects.Add(new Project("Проект" + 1));
            TreeViewElements.ItemsSource = projects;
            if (True != null)
                True(projects.Last().Name);

        }
        private void Confirm(object sender, RoutedEventArgs e)
        {

            var x = projects[0].SelectedElement.coefficients[0].Value;
        }
        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            var source = projects[0].SelectedElement.coefficients;
            //source.Add(projects[0].SelectedElement.Lambda);
            GridKoeff.ItemsSource = source;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //GridProp.Items.Refresh();
        }
        private void ParamValueIsSelected(object sender, RoutedEventArgs e)
        {
            GridProp.Items.Refresh();
        }
        private void Method_MonteKarlo(object sender, RoutedEventArgs e)
        {
            MonteKarlo monteKarlo = new MonteKarlo(projects[0].Lambdas, 100000000);
            projects[0].Results.Clear();
            projects[0].Results.Add(new MTF(monteKarlo.MeanTimeToFailure));
            projects[0].Results.Add(new FR(monteKarlo.FailureRate));
            projects[0].Results.Add(new RNF(monteKarlo.FailureRate, 10000));
            GridProp.ItemsSource = projects[0].Results;

            GridProp.Items.Refresh();
        }
        private void ClickProject(object sender, MouseButtonEventArgs e)
        {
            GridProp.ItemsSource = projects[0].Property.Concat(projects[0].Results);
            GridProp.Items.Refresh();
        }

        private void ReNameElement(object sender, RoutedEventArgs e)
        {
            projects[0].SelectedElement.IsReName = true;
        }

        private void ReNameBox_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "ras";
            openFileDialog.Filter = "Файлы ReliabilityAnalysis|*.ras";
            openFileDialog.ShowDialog();
            var project = Project.OpenProject(openFileDialog.FileName);
            if (project != null)
            {
                projects.Add(project);
                TreeViewElements.ItemsSource = projects;
            }
            else
            {
                MessageBox.Show("Ошибка чтения файла");
            }
        }
        private void SaveAsFileDialog(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "ras";
            saveFileDialog.Filter = "Файлы ReliabilityAnalysis|*.ras";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
                if (projects[0].SaveProjectAs(saveFileDialog.FileName) == false)
                {
                    MessageBox.Show("Ошибка записи в файл");
                }
        }
        private void DeleteElement(object sender, RoutedEventArgs e)
        {
            projects[0].Elements.Remove(projects[0].SelectedElement);
        }
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (projects[0].SaveProject() == false)
            {
                this.SaveAsFileDialog(null, null);
            }
            else
            {
                MessageBox.Show("Проект сохранен");
            }
        }
        private void ExitFromApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
