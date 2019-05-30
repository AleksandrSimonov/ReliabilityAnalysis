using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliabilityAnalysis.Scheme.ElementsOfDataGrid
{
    /// <summary>
    /// Время эксплуатации схемы
    /// </summary>
    [Serializable]
    public class TimeOfExpluatation : ElementOfDataGrid
    {
        double timeInHours = 24 * 365 * 5;
        public TimeOfExpluatation()
        {
            Param = "Время эксплуатации";
        }
        /// <summary>
        /// Значение времени эксплуатации схемы
        /// </summary>
        public override double Value
        {
            get
            {
                return timeInHours;
            }
            set
            {
                timeInHours = value;
            }
        }

        public Project Project
        {
            get => default(Project);
            set
            {
            }
        }
    }
}
