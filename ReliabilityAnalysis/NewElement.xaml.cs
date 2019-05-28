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
using ReliabilityAnalysis.DataBase;

namespace ReliabilityAnalysis
{
    /// <summary>
    /// Логика взаимодействия для NewElement.xaml
    /// </summary>
    public partial class NewElement : Window
    {
        private Node item;
        private Project project;
        public NewElement(Project project, Node item)
        {
            InitializeComponent();

            this.item = item;
            this.project = project;
            Designation.Text = item.DefDesign+(project.Elements.Count+1).ToString();
        }

        private void AddEriToProject(object sender, RoutedEventArgs e)
        {
            item.Designation = Designation.Text;
            if (project.Contains(item.Designation) == false)
            {
                var el = new Element(item,project.Property[4].Value);
                project.Add(el);
                this.Close();
            }
            else
                MessageBox.Show("ЭРИ с таким обозначением уже есть в проекте!");
        }
    }
}
