using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ReliabilityAnalysis.Scheme;

namespace ReliabilityAnalysis.DataBase
{
    [Serializable]
    public class Element
    {

        public Element(Node item)
        {
            IDElement = item;
            Class = Tables.Class.GetName(item.ID_Class);
            Scroll = Tables.Scroll.GetName(item.ID_Class);
            Type = Tables.Type.GetName(item.ID_Type);
            Designation = item.Designation;
            LambdaBasic = new LambdaBasic(Tables.Lambda.GetLambdaValue(item));
            //MathModel = Tables.GetMathModel(item);
            this.coefficients = Tables.GetCoefficients(item);
            Name = item.Name;
            this.IsReName = false;
        }
        public Node IDElement { get; set; }
        public K SelectedCoefficient { get; set; }
        public string Designation { get; set; }
        public bool IsReName{ get; set; }
        public string Name { get; }
        public string Class { get; }
        public string Scroll { get; }
        public string Type { get; }
        public LambdaBasic LambdaBasic
        {
            get;              
        }
        public LambdaExp LambdaExp
        {
            get
            {
                double lambda = Convert.ToDouble(LambdaBasic.SelectedParamValue);
              
                return new LambdaExp(lambda, this);
            }
        }
        public double Reliability { get; }
        public ObservableCollection<PRY> ElementOfGrid
        {
            set { }
            get
            {
                var elg = new ObservableCollection<PRY>();

                foreach(var coeff in coefficients)
                    if ((coeff.ID_KIndex != 2) && (coeff.ID_KIndex != 3))
                        elg.Add(coeff);
                
                elg.Add(LambdaBasic);
                elg.Add(LambdaExp);

                return elg;
            }
        }
        public ObservableCollection<K> coefficients { set; get; }
        public string MathModel { get; set; }
    }
}
