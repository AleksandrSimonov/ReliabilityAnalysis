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

        public double MeanTimeToFailure { get; } //Средняя наработка на отказ
        public double FailureRate //Интенсивность отказов
        {
            get
            {
                return 1.0 / MeanTimeToFailure;
            }
        }
        private double MTF()// Средняя наработка на отказ
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

        public MonteKarlo(List<double> lambdas, int Count)
        {
            this.lambdas = lambdas;
            this.Count = Count;
            random = new TRandom();
            MeanTimeToFailure = MTF();


        }


    }
}
