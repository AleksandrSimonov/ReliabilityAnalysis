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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SqliteORM;
using System.Collections.ObjectModel;
using ReliabilityAnalysis.Scheme;

namespace ReliabilityAnalysis.DataBase
{
    /// <summary>
    /// Коэффициент математической модели элемента
    /// </summary>
    [Serializable]
    public class Coefficient: ElementOfDataGrid
    {
        private double value = 0;
        public Node ID { get; set; }
        public string Name { get; set; }
        public double Temperature { get; set; }
        public string ParamName { get; set; }
        public string Discription { get; set; }
        public string ParamDiscription { get; set; }
        new public string Param
        {
            get
            {
                if (ParamDiscription != null)
                    return ParamDiscription + " " + ParamName;
                else
                    return Discription + " " + ParamName;
            }
            set { }
        }
        string item = "";
        new public string SelectedParamValue
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }
        public List<string> ComboBoxItemsList
        {
            get
            {
                    var listInfo = new List<string>();
                    foreach (var info in Info)
                        listInfo.Add(info.Discription);
                    return listInfo;
            }
           
        }
        public Int64 ID_KIndex { get; set; }
        public override double Value
        {
            get
            {
                if ((SelectedParamValue != "") && (ID != null))
                {
                    try
                    {
                        var x = Tables.GetCoefficientValue(this);
                        return x;
                    }catch(ArgumentException ex)
                    {
                        throw ex;
                    }
                }
                return 0;
            }
        }
        public new string KValue
        {
            get
            {
                return Value.ToString();
            }
        }
        /// <summary>
        /// Инициализация коэффициента
        /// </summary>
        /// <param name="ID_Kindex">Идентификационный номер коэффициента в БД</param>
        public Coefficient(Int64 ID_Kindex)
        {
            this.ID_KIndex = ID_Kindex;
        }
        public Coefficient() { }
        public override int GetHashCode()
        {
            return this.ID_KIndex.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            try
            {
                return ID_KIndex.Equals(((Coefficient)obj).ID_KIndex);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Element Element
        {
            get => default(Element);
            set
            {
            }
        }
    }
}
