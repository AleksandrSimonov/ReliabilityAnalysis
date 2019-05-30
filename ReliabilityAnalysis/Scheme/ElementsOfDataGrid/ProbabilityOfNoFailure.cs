using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliabilityAnalysis.Scheme.ElementsOfDataGrid
{

    /// <summary>
    ///  Вероятность безотказной работы проекта
    /// </summary>
    [Serializable]
    public class ProbabilityOfNoFailure : ElementOfDataGrid
    {
        public long Time { get; set; }
        private double lambda;
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lambda">Интенсивность отказов всей схемы</param>
        /// <param name="time">Время эксплуатации всей схемы</param>
        public ProbabilityOfNoFailure(double lambda, long time)
        {
            Param = "Вероятность безотказной работы";
            Time = time;
            this.lambda = lambda;
        }
        /// <summary>
        /// Значение интенсивности отказов схемы
        /// </summary>
        public override double Value
        {
            get
            {
                return Math.Exp(-lambda * Time);
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
