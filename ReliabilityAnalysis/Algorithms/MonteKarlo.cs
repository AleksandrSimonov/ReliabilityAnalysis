using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random;

namespace ReliabilityAnalysis.Algorithms
{
    public class MonteKarlo
    {
        private List<double> lambdas;
        private int Count;
        TRandom random;

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

        /// <summary>
        /// Средняя наработка до отказ
        /// </summary>
        public double MeanTimeToFailure { get; }

        /// <summary>
        /// Интенсивность отказов
        /// </summary>
        public double FailureRate
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

        /// <summary>
        /// Средняя наработка до отказа
        /// </summary>
        /// <returns></returns>
        private double MTF()
        {
            double time = 0, time2, atime = 0;

            int count = lambdas.Count;
            for (int i = 0; i < Count; i++)
            {
                time = random.Exponential(lambdas[0]);
                for (int j = 1; j < count; j++)
                {
                    time2 = random.Exponential(lambdas[j]);
                    if (time2 < time)
                        time = time2;
                }
                atime += time;
            }
            return atime / Count;

        }

        /// <summary>
        /// Анализ надежности методом Монте-Карло
        /// </summary>
        /// <param name="lambdas">Список эксплуатационных интенсивностей отказов всех Эри, входящих в проект</param>
        /// <param name="Count">Число опытом в методе</param>
        /// <param name="gammaPercent">Гамма-процент</param>
        public MonteKarlo(List<double> lambdas, int Count, double gammaPercent)
        {
            this.lambdas = lambdas;
            this.Count = Count;
            random = new TRandom();
            MeanTimeToFailure = MTF();
            GammaPercent = gammaPercent;
        }


    }
}
