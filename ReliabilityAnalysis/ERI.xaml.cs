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
using System.Collections.ObjectModel;
using ReliabilityAnalysis.DataBase;
using ReliabilityAnalysis.DataBase;

namespace ReliabilityAnalysis
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ERI : Window
    {
        ObservableCollection<Node> nodes;

        Project project;
        public ERI(Project project)
        {
            InitializeComponent();
         
            nodes = Tables.ShowBD();
            this.project = project;
            TreeView.ItemsSource = nodes;
        }

        public MainWindow MainWindow
        {
            get => default(MainWindow);
            set
            {
            }
        }

        private void SelectItem(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (Node) TreeView.SelectedItem;

            if ((item.Nodes == null)||(item.Nodes.Count==0))
            {
                var newEl = new NewElement(project, item);
                newEl.ShowDialog();
            }
           
        }
    }
  
}
