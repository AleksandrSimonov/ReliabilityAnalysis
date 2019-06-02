using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliabilityAnalysis.Scheme.ElementsOfDataGrid
{
    /// <summary>
    /// Температура среды в градусах цельсия
    /// </summary>
    [Serializable]
    public class Temperature : ElementOfDataGrid
    {
        double temperature = 50;

        public override double Value
        {
            get
            {
               return temperature;
            }
            set
            {
                temperature = value;
            }
        }

        public Project Project
        {
            get => default(Project);
            set
            {
            }
        }

        public Temperature()
        {
            Param = "Температура среды, °С";
        }
    }
}
