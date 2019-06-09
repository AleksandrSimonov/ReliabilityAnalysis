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
    /// Логика взаимодействия для AlgorithmAccuracy.xaml
    /// </summary>
    public partial class AlgorithmAccuracy : Window
    {
        MainWindow mw;
        public AlgorithmAccuracy(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
        }

        private void Method_MonteKarlo(object sender, RoutedEventArgs e)
        {
           mw.Accuracy = (int)Accuracy.Value;
            this.Close();
        }
    }
}
