using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ciloci.Flee;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using Brushes = System.Windows.Media.Brushes;

namespace ReliabilityAnalysis.GraphicView
{

   static class GraphicView
    {

        static public SeriesCollection Reliability(double lambda)
        {
            /* var func = new CartesianMapper<double>();

             var pow = 7;//(int)Math.Floor(Math.Pow(Math.Log(Math.Pow(10, -11)) / -lambda, 0.1));

             func.X((value, index) => value * Math.Pow(10, pow));
             func.Y((value, index) => Math.Exp(-lambda * value * Math.Pow(10, pow)));*/
           int Base = 10;

            var mapper = Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log(point.X, Base)) //a 10 base log scale in the X axis
                .Y(point => point.Y);

            var collection = new List<ObservablePoint>();


            for (double i = 0; i < 10; i+=0.1)
                collection.Add(new ObservablePoint(Math.Pow(10, i), Math.Exp(-lambda * Math.Pow(10, i))));

            return new SeriesCollection(mapper){

                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(collection)
                 ,
               Title="P(time)=",
               Uid="time",
                PointGeometry = null

                },
            };
            //Formatter = value => Math.Pow(Base, value).ToString("N");
        }
        static  public SeriesCollection  Histograma(List<double> lambdas)
        {
            return new SeriesCollection{

                new ColumnSeries
                {
                    
                    Values = new ChartValues<double>(lambdas),
               Title="λ=",



                },
            };

        }
       
    }
}
