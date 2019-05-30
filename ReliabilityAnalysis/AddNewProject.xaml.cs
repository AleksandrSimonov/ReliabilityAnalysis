using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReliabilityAnalysis
{
    /// <summary>
    /// Логика взаимодействия для AddNewProject.xaml
    /// </summary>
    public partial class AddNewProject : Window
    {
        MainWindow mainWindow;

        public AddNewProject(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
        }

        public MainWindow MainWindow
        {
            get => default(MainWindow);
            set
            {
            }
        }

        private void AddNewProjectClick(object sender, RoutedEventArgs e)
        {
            mainWindow.ProjectNumber = 1;
            mainWindow.Projects.Add(new Project(projectName.Text));
            mainWindow.TreeViewElements.ItemsSource = mainWindow.Projects;
            this.Close();
        }
    }
}
