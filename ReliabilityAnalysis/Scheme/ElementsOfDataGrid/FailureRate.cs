using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliabilityAnalysis.Scheme.ElementsOfDataGrid
{
    /// <summary>
    /// Интенсивность отказов всего проекта
    /// </summary>
    [Serializable]
    public class FailureRate : ElementOfDataGrid
    {
        public override bool IsReadOnly { get { return false; } }

        public Project Project
        {
            get => default(Project);
            set
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fr">Интенсивность отказов</param>
        public FailureRate(double fr)
        {
            Param = "Интенсивность отказов, 1/ч";
            Value = fr;
        }

    }
}
