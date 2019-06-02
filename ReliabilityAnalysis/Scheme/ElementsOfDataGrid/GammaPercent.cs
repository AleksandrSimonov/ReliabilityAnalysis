using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliabilityAnalysis.Scheme.ElementsOfDataGrid
{
    /// <summary>
    /// Гамма-процент
    /// </summary>
    [Serializable]
    public class GammaPercent : ElementOfDataGrid
    {
        double gammaPercent = 95;

        public GammaPercent()
        {
            Param = "Гамма-процент, %";
        }
        public override double Value
        {
            get
            {
                return gammaPercent;
            }
            set
            {

            }
        }
        new public string SelectedParamValue
        {
            get
            {
                return gammaPercent.ToString();
            }
            set
            {
                if ((Convert.ToInt32(value) < 0) || (Convert.ToInt32(value) > 100))
                {
                    gammaPercent = 0;
                    throw new ArgumentOutOfRangeException();
                }
                gammaPercent = (Convert.ToInt32(value));
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
