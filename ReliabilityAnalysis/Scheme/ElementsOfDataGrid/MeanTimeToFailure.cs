using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliabilityAnalysis.Scheme.ElementsOfDataGrid
{
    /// <summary>
    /// Средняя наработка на отказ
    /// </summary>
    [Serializable]
    public class MeanTimeToFailure: ElementOfDataGrid
    {
        public override bool IsReadOnly { get { return false; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mtf">Значение средней наработки на отказ</param>
        public MeanTimeToFailure(double mtf)
        {
            Param = "Средняя наработка на отказ";
            Value = mtf;

        }

    }
}
