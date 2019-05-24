using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliabilityAnalysis.DataBase;

namespace ReliabilityAnalysis.Scheme
{
    [Serializable]
    public abstract class PRY
    {
        public virtual string SelectedParamValue { get; set; }
        public List<Tables.Info> Info { get; set; }        
        public string Param { get; set; }
        public double Value { get; set; }
        public virtual bool IsReadOnly
        {
            get
            {
                return true;
            }
        }
        public PRY()
        {
            Info = new List<Tables.Info>();
        
        }

    }
    [Serializable]
    public class LambdaExp : PRY
    {
        private Element el;
        new public string SelectedParamValue
        {
            get
            {
                var lambda = Value;
                foreach (var k in el.coefficients)
                {
                    k.ID = el.IDElement;
                    lambda *= k.Value;
                }
                return lambda.ToString();
            }
            private set { }
        }
        public override bool IsReadOnly { get { return false; } }
        public LambdaExp(double value, Element el)
        {
            this.el = el;
            Param = "Эксплуатационная интенсивность отказов";
            Value = value;
            SelectedParamValue = value.ToString();
           
        }
    }
    [Serializable]
    public class LambdaBasic : PRY
    {
        public LambdaBasic(double value)
        {
            Param = "Базовая интенсивность отказов";
            Value = value;
            SelectedParamValue = value.ToString();
        }
    }
    [Serializable]
    public class MTF: PRY
    {
        public override bool IsReadOnly { get { return false; } }
        public MTF(double value):base()
        {
            Param = "Средняя наработка на отказ";
            SelectedParamValue = value.ToString();

        }

    }
    [Serializable]
    public class FR : PRY
    {
        public override bool IsReadOnly { get { return false; } }
        public FR(double value):base()
        {
            Param = "Интенсивность отказов";
            SelectedParamValue = value.ToString();
        }

    }
    [Serializable]
    public class RNF : PRY
    {
       public long Time { get; set; }
        double lambda;
        public override bool IsReadOnly { get { return false; } }
        public RNF(double lambda, long time ):base()
        {
            Param = "Вероятность безотказной работы";
            SelectedParamValue = Math.Exp(-lambda*time).ToString();
            Time = time;
            this.lambda = lambda;
        }
        new public string SelectedParamValue
        {
            get
            {
                return Math.Exp(-lambda * Time).ToString();
            }
            private set { }
        }
    }
    [Serializable]
    public class Time : PRY
    {
        long timeInHours=24*360*5;
        public Time() : base()
        {
            Param = "Время эксплуатации";
        }
        override public string SelectedParamValue
        {
            get
            {
                return timeInHours.ToString();
            }
            set
            {
                timeInHours = Convert.ToInt64(value);
            }
        }
    }
    [Serializable]
    public class GammaPercent : PRY
    {
        int gammaPercent=95;

        public GammaPercent() 
        {
            Param = "Гамма-процент";
        }
        override public string SelectedParamValue
        {
            get
            {
                return gammaPercent.ToString();
            }
            set
            {
                gammaPercent = Convert.ToInt32(value);
                if ((gammaPercent < 0) || (gammaPercent > 100))
                {
                    gammaPercent =0;
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    [Serializable]
    public class Temperature : PRY
    {
        double temperature=50;

        public Temperature()
        {
            Param = "Температура среды";
        }
        override public string SelectedParamValue
        {
            get
            {
                return temperature.ToString();
            }
            set
            {
                temperature = Convert.ToDouble(value);
            }
        }
    }

}
