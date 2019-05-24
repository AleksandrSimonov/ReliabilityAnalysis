using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ReliabilityAnalysis.DataBase
{
    [Serializable]
   public class Node
    {
        public  string Name { get; set; }

        public string Designation { get; set; }
        public Int64 ID_Class { get; set; }
        public Int64 ID_Scroll { get; set; }
        public Int64 ID_Type { get; set; }
        public string DefDesign { get; set; }
        private ObservableCollection<Node> nodes;
        public ObservableCollection<Node> Nodes
        {
            set
            {
                nodes = value;
                foreach (var n in nodes)
                    n.DefDesign = DefDesign;
            }
            get
            {
                return nodes;
            }
        }
    }
}
