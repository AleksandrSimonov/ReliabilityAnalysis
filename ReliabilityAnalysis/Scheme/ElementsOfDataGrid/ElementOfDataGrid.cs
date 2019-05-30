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
        private string selectedParamValue;
        /// <summary>
        /// Значение столбца "Свойства"
        /// </summary>
        public string SelectedParamValue
        {
            get
            {
                try
                {
                    var x = Value.ToString();
                    return x;
                }catch(ArgumentException ex)
                {
                    return "undefind";
                }
            }
            set
            {
                try
                {
                    Value = Convert.ToDouble(value);
                }catch(FormatException ex)
                {
                    IsCorrect = false;
                   selectedParamValue = value;
                }
            }
        }
        public bool IsCorrect
        {
            get
            {
                try
                {
                    var x = Value;
                    //Value = Convert.ToDouble(selectedParamValue);
                    return true;
                }
                catch (ArgumentException ex)
                {
                    return false;
                }
            }
            private set { }
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
