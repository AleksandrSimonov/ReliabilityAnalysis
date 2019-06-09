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
using ReliabilityAnalysis.Scheme.ElementsOfDataGrid;
using LiveCharts;
using LiveCharts.Wpf;
using ReliabilityAnalysis.GraphicView;


namespace ReliabilityAnalysis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int ProjectNumber { get; set; }

        public SeriesCollection SerieFunc { get; set; }
        public SeriesCollection SerieHis { get; set; }
        public Func<double, string> Formatter { get; set; }
        public ObservableCollection<string> LabelsHis { get; set; }
        public int Accuracy { get; set; }



        public ObservableCollection<Project> Projects { get; set; }
       
        public Project SelectedProject
        {
            get
            {
                if (Projects != null)
                    return null;
                return Projects[0];
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
            Tables.StringConnection = @"data source=AppData\data.db"; 


            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Projects = new ObservableCollection<Project>();
            Accuracy = 1;

        }
        private void ShowEriList(object sender, RoutedEventArgs e)
        {
            ERI eri = new ERI(Projects[0]);
            eri.ShowDialog();
        }
        private void ClickTextDesignation(object sender, MouseEventArgs e)
        {
            Projects[0].SelectElement(((TextBlock)sender).Text);
            var source = Projects[0].SelectedElement.ElementOfGrid;
            GridProp.ItemsSource = source;
            AppMenu.Items.Refresh();
        }
        private void CreateNewProject(object sender, RoutedEventArgs e)
        {
            if (Projects.Count != 0)
                MessageBox.Show("Существующий проект будет закрыт!");

            var newProject = new AddNewProject(this);
            newProject.ShowDialog();

        }
        private void Confirm(object sender, RoutedEventArgs e)
        {
            var x = Projects[0].SelectedElement.Сoefficients[0].Value;
        }
        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            var source = Projects[0].SelectedElement.Сoefficients;
            //source.Add(Projects[0].SelectedElement.Lambda);
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
            try
            {
                MonteKarlo monteKarlo = new MonteKarlo(Projects[0].Lambdas, Convert.ToInt32( 100*Math.Pow(10, Accuracy)), Projects[0].Property[3].Value);
                Projects[0].Results.Clear();
                Projects[0].Results.Add(new MeanTimeToFailure(monteKarlo.MeanTimeToFailure));
                Projects[0].Results.Add(new FailureRate(monteKarlo.FailureRate));
                Projects[0].Results.Add(new ProbabilityOfNoFailure(monteKarlo.FailureRate, (long) Projects[0].Property[2].Value));
                Projects[0].Results.Add(new GammaPercentTimeToFailure(monteKarlo.GammaPercentTimeToFailure));
                GridProp.ItemsSource = Projects[0].Property.Concat(Projects[0].Results);
;
                SerieFunc = GraphicView.GraphicView.Reliability(monteKarlo.FailureRate);
                SerieHis = GraphicView.GraphicView.Histograma(Projects[0].Lambdas);
                LabelsHis = new ObservableCollection<string>( Projects[0].Elements.Select((el) => { return el.Designation; }));
                Formatter = value => Math.Pow(10, value).ToString("N");
                ReliabilityGrid.Visibility = Visibility.Visible;
                DataContext = this;
                GridProp.Items.Refresh();

            }catch(ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ClickProject(object sender, MouseButtonEventArgs e)
        {
            GridProp.ItemsSource = Projects[0].Property.Concat(Projects[0].Results);
            GridProp.Items.Refresh();
        }
        private void ReNameElement(object sender, RoutedEventArgs e)
        {
            Projects[0].SelectedElement.IsReName = true;
        }
        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            if(Projects.Count!=0)
                MessageBox.Show("Существующий проект будет закрыт!");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "ras";
            openFileDialog.Filter = "Файлы ReliabilityAnalysis|*.ras";
            openFileDialog.ShowDialog();



            if (openFileDialog.FileName != "")
            {
                var project = Project.OpenProject(openFileDialog.FileName);
                if (project != null)
                {
                    Projects.Clear();
                    Projects.Add(project);
                    TreeViewElements.ItemsSource = Projects;
                    GridProp.ItemsSource = Projects[0].Property.Concat(Projects[0].Results);
                    GridProp.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Ошибка чтения файла");
                }
            }
        }
        private void SaveAsFileDialog(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "ras";
            saveFileDialog.Filter = "Файлы ReliabilityAnalysis|*.ras";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
                if (Projects[0].SaveProjectAs(saveFileDialog.FileName) == false)
                {
                    MessageBox.Show("Ошибка записи в файл");
                }
        }
        private void DeleteElement(object sender, RoutedEventArgs e)
        {
            Projects[0].Elements.Remove(Projects[0].SelectedElement);
        }
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (Projects[0].SaveProject() == false)
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
        private void Markov_Method(object sender, RoutedEventArgs e)
        {
            try
            {
                var markov = new Markov(Projects[0].Lambdas, Projects[0].Property[3].Value);
                Projects[0].Results.Clear();
                Projects[0].Results.Add(new MeanTimeToFailure(markov.MeanTimeToFailure));
                Projects[0].Results.Add(new FailureRate(markov.FailureRate));
                Projects[0].Results.Add(new ProbabilityOfNoFailure(markov.FailureRate,(long) Projects[0].Property[2].Value));
                Projects[0].Results.Add(new GammaPercentTimeToFailure(markov.GammaPercentTimeToFailure));
                GridProp.ItemsSource = Projects[0].Results;

                SerieFunc = GraphicView.GraphicView.Reliability(markov.FailureRate);
                SerieHis = GraphicView.GraphicView.Histograma(Projects[0].Lambdas);
                LabelsHis = new ObservableCollection<string>(Projects[0].Elements.Select((el) => { return el.Designation; }));
                Formatter = value => Math.Pow(10, value).ToString("N");
                ReliabilityGrid.Visibility = Visibility.Visible;
                DataContext = this;

                GridProp.Items.Refresh();
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.ParamName);
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            /*{
                Point p00 = new Point(30, 10);
                double step = grid1.ActualWidth / 1.3 / 10.0;
                grid1.Children.Clear();
                Line vertL = new Line();
                vertL.X1 = p00.X;
                vertL.Y1 = p00.Y;
                vertL.X2 = p00.X;
                vertL.Y2 = grid1.ActualHeight/1.7;
                vertL.Stroke = Brushes.Black;
                grid1.Children.Add(vertL);

                Line horL = new Line();
                horL.X1 = vertL.X2;
                horL.Y1 = vertL.Y2;
                horL.X2 = grid1.ActualWidth/1.3;
                horL.Y2 = vertL.Y2;
                horL.Stroke = Brushes.Black;
                grid1.Children.Add(horL);

                TextBlock tb;
             
                for (byte i = 0; i*step < grid1.ActualWidth / 1.3; i++)
                {
                    Line a = new Line();
                    a.X1 = i * step;
                    a.X2 = i * step;
                    a.Y1 = grid1.ActualHeight / 1.7+5;
                    a.Y2 = grid1.ActualHeight / 1.7-5;
                    a.Stroke = Brushes.Black;
                    grid1.Children.Add(a);

                    tb = new TextBlock();
                    tb.Text = i.ToString();
                    tb.Margin = new Thickness(p00.X-15, grid1.ActualHeight / 1.7 - i * step, 10, 10);
                    grid1.Children.Add(tb);


           
                }

                for (byte i = 1; i * step < grid1.ActualHeight / 1.7; i++)
                {
                    Line a = new Line();
                    a.X1 = p00.X-5;
                    a.X2 = p00.X+5;
                    a.Y1 = i * step;
                    a.Y2 = i*step;
                    a.Stroke = Brushes.Black;
                    grid1.Children.Add(a);
                }

                Polyline vertArr = new Polyline();
                vertArr.Points = new PointCollection();
                vertArr.Points.Add(new Point(p00.X-5, p00.Y+10));
                vertArr.Points.Add(new Point(p00.X, p00.Y));
                vertArr.Points.Add(new Point(p00.X+5, p00.Y+10));
                vertArr.Stroke = Brushes.Black;
                grid1.Children.Add(vertArr);

                Polyline horArr = new Polyline();
                horArr.Points = new PointCollection();
                horArr.Points.Add(new Point(grid1.ActualWidth / 1.3-10, grid1.ActualHeight / 1.7-5));
                horArr.Points.Add(new Point(grid1.ActualWidth / 1.3, grid1.ActualHeight / 1.7));
                horArr.Points.Add(new Point(grid1.ActualWidth / 1.3 - 10, grid1.ActualHeight / 1.7 +5));
                horArr.Stroke = Brushes.Black;
                grid1.Children.Add(horArr);


            }*/
        }
        private void ClickAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данное программное обеспечение выполнили студенты КНИТУ-КАИ: Симонов А.В. и Хабибулина Л.М.");
        }
        private void ClickHelp(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"Руководство пользователя.docx");
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var a = new AlgorithmAccuracy(this);
            a.Show();
        }
    }
}
