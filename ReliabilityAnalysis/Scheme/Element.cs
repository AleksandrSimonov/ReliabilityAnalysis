using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ReliabilityAnalysis.Scheme;
using ReliabilityAnalysis.Scheme.ElementsOfDataGrid;

namespace ReliabilityAnalysis.DataBase
{
    /// <summary>
    /// Элемент схемы
    /// </summary>
    [Serializable]
    public class Element
    {
        /// <summary>
        /// Инициализация элемента схемы
        /// </summary>
        /// <param name="item">ID элемента в БД и его описание в проекте</param>
        /// <param name="temperature">Температура элемента</param>
        public Element(Node item, ElementOfDataGrid temperature)
        {

            IDElement = item;
            Temperature =temperature.Value;
            Class = Tables.Class.GetName(item.ID_Class);
            Scroll = Tables.Scroll.GetName(item.ID_Class);
            Type = Tables.Type.GetName(item.ID_Type);
            Designation = item.Designation;
            LambdaBasic = new LambdaBasic(Tables.Lambda.GetLambdaValue(item));
            //MathModel = Tables.GetMathModel(item);
            this.Сoefficients = Tables.GetCoefficients(item, temperature);
            Name = item.Name;
            this.IsReName = false;
        }
        public double Temperature { get; set; }
        public Node IDElement { get; set; }
        public Coefficient SelectedCoefficient { get; set; }
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
        public ObservableCollection<ElementOfDataGrid> ElementOfGrid
        {
            set { }
            get
            {
                var elg = new ObservableCollection<ElementOfDataGrid>();

                foreach(var coeff in Сoefficients)
                    if ((coeff.ID_KIndex != 2) && (coeff.ID_KIndex != 3))
                        elg.Add(coeff);
                
                elg.Add(LambdaBasic);
                elg.Add(LambdaExp);

                return elg;
            }
        }
        /// <summary>
        /// Список коэффициентов элемента
        /// </summary>
        public ObservableCollection<Coefficient> Сoefficients { set; get; }
        public string MathModel { get; set; }
    }
}
