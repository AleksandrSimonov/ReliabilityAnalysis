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

        public Markov(List<double> lambdas)
        {
            this.lambdas = lambdas;
            MeanTimeToFailure = MTF();
        }


    }
}
