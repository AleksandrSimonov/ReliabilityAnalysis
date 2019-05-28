using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliabilityAnalysis.DataBase;

namespace ReliabilityAnalysis.Scheme
{
    /// <summary>
    /// Представляет описание элемента таблицы
    /// </summary>
    [Serializable]
    public abstract class ElementOfDataGrid
    {
        /// <summary>
        /// Значение столбца "Свойства"
        /// </summary>
        public string SelectedParamValue
        {
            get
            {
                var x = Value.ToString();
                return x;
            }
            set
            {
                Value = Convert.ToDouble(value);
            }
        }
        public string KValue
        {
            get
            {
                var x = Value.ToString();
                return x;
            }
        }
        public List<Tables.Info> Info { get; set; }
        /// <summary>
        /// Значение столбца "Наименование"
        /// </summary>
        public string Param { get; set; }
        public virtual double Value { get; set; }
        /// <summary>
        /// Разрешено ли редактирование
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                return true;
            }
        }
        public ElementOfDataGrid()
        {
            Info = new List<Tables.Info>();
        }

    }
}
