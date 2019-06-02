using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ciloci.Flee;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Media;

namespace ReliabilityAnalysis.GraphicView
{

    class GraphicView
    {
        ExpressionContext context;
        List<string> s;

        public GraphicView()
        {
            context = new ExpressionContext();
            context.Imports.AddType(typeof(Math));
            s = context.Variables.Keys.ToList();
        }

        public List<PointF> PointsOfFunctions(string expression, double left, double right, double step)
        {
            IDynamicExpression eDynamic;
            var points = new List<PointF>();

            for (double x = left; x < right; x += step)
            {
                if (x + step > right)
                    x = right;
                context.Variables["x"] = x;
                eDynamic = context.CompileDynamic(expression);
                points.Add(new PointF((float)x, (float)eDynamic.Evaluate()));

            }
            return points;
        }
        public List<System.Windows.Shapes.Rectangle> FailureRateDistribution(List<double> lambdas)
        {
            var min = lambdas.Min();
            var max = lambdas.Max();
            var step = (max - min) / 10.0;

            var rectangles = new List<System.Windows.Shapes.Rectangle>();

            for (int i = 0; i < lambdas.Count; i++)
            {
                rectangles.Add(new System.Windows.Shapes.Rectangle() { Width = 20, Height = lambdas[i] / step, Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 0, 0)) });
            }
            return rectangles;
        }
        /*  public  class FuncPoint
            {
                public double X { get; set; }
                public double Y { get; set; }

                public FuncPoint(double x, double y)
                {
                    X = x;
                    Y = y;
                }
            }*/
    }
}
