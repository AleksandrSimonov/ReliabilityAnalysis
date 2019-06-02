using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ReliabilityAnalysis
{
    public class Markov
    {
        private List<double> lambdas;

        /// <summary>
        /// Гамма-процент
        /// </summary>
        public double GammaPercent { get; }
        /// <summary>
        /// Гамма-процентная наработка до отказа
        /// </summary>
        public double GammaPercentTimeToFailure
        {
            get
            {
                return -MeanTimeToFailure * Math.Log(GammaPercent / 100.0);
            }
        }
        public double MeanTimeToFailure { get; } //Средняя наработка на отказ
        public double FailureRate //Интенсивность отказов
        {
            get
            {
                return 1.0 / MeanTimeToFailure;
            }
        }

        public Project Project
        {
            get => default(Project);
            set
            {
            }
        }

        private double MTF()// Средняя наработка на отказ
        {
            return 1.0 / lambdas.Sum();
        }

        /// <summary>
        /// Анализ надежности методом Марковских цепей
        /// </summary>
        /// <param name="lambdas">Список эксплуатационных интенсивностей отказов всех Эри, входящих в проект</param>
        /// <param name="gammaPercent">Гамма-процент</param>
        public Markov(List<double> lambdas, double gammaPercent)
        {
            this.lambdas = lambdas;
            MeanTimeToFailure = MTF();
            GammaPercent = gammaPercent;
        }


    }
}
