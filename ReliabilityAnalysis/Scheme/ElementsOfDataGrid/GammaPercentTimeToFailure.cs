using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliabilityAnalysis.Scheme.ElementsOfDataGrid
{
    /// <summary>
    /// Гамма-процентная наработка до отказа
    /// </summary>
    [Serializable]
    public class GammaPercentTimeToFailure : ElementOfDataGrid
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
        /// <param name="gptf">Значение гамма-процентной наработки до отказа</param>
        public GammaPercentTimeToFailure(double gptf)
        {
            Param = "Гамма-процентная наработка до отказа, ч";
            Value = gptf;

        }
    }
}
