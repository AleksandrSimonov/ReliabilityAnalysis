﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliabilityAnalysis.DataBase;

namespace ReliabilityAnalysis.Scheme.ElementsOfDataGrid
{
    /// <summary>
    /// Эксплуатационная интенсивность отказов
    /// </summary>
    [Serializable]
    public class LambdaExp : ElementOfDataGrid
    {
        private Element selectedElement;
        private double lambdaBasic;
        public override double Value
        {
            get
            {
                var lambda = lambdaBasic;
                foreach (var k in selectedElement.Сoefficients)
                {
                    try
                    {
                        k.ID = selectedElement.IDElement;
                        lambda *= k.Value;
                    }catch(ArgumentException ex)
                    {
                        throw ex;
                    }
                }
                return lambda;
            }
        }
        public override bool IsReadOnly { get { return false; } }

        public Element Element
        {
            get => default(Element);
            set
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lambdaBasic">Базовая интенсивность отказов</param>
        /// <param name="selectedElement">Выбранный элемент проекта</param>
        public LambdaExp(double lambdaBasic, Element selectedElement)
        {
            this.selectedElement = selectedElement;
            Param = "Эксплуатационная интенсивность отказов, 1/ч";
            this.lambdaBasic = lambdaBasic;
        }
    }
}
