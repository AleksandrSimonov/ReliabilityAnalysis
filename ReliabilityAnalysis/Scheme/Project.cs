using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SqliteORM;
using ReliabilityAnalysis.DataBase;
using ReliabilityAnalysis.Scheme;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ReliabilityAnalysis
{

[Serializable]
 public   class Project
    {
        private ObservableCollection<PRY> property;
        Element selectedElement;
        public ObservableCollection<Element> Elements{ set; get; }
        public ObservableCollection<PRY> Results { set; get; }
        public ObservableCollection<PRY> Property
        {
            get
            {
                return property;
            }
        }
        public List<double> Lambdas
        {
            get
            {
                var count = Elements.Count;
                double lambda;
                var lambdas = new List<double>();
                for (int i = 0; i < count; i++)
                {
                    lambda = Convert.ToDouble(Elements[i].LambdaExp.SelectedParamValue);
                    if (lambda == 0)
                        return null;
                    lambdas.Add(lambda);
                }
                return lambdas;
            }
        }
        public long Time { set; get; }
        public double Temperature { get; set; }
        public Element SelectedElement
        {
            get
            {
                return selectedElement;
            }
        }
        public string Name { get; set; }
        public string FileName { get; set; }

        public Element this[string desig]
        {
            get
            {
                return Elements.First(el => el.Designation == desig);
            }
            
        }

        public bool SaveProject()
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, this);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SaveProjectAs(string path)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, this);
                    FileName = path;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static Project OpenProject(string path)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    return (Project)formatter.Deserialize(fs);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public void Add(Element el)
        {

            el.coefficients.Add((K)property[0]);
            el.coefficients.Add((K)property[1]);

            Elements.Add(el);
        }
        public bool Contains(string design)
        {
            if (Elements.FirstOrDefault(el => el.Designation == design) == null)
                return false;
            return true;
        }
        public void SelectElement(string design)
        {
            selectedElement = this[design];
        }

        public Project(string name)
        {
            Elements = new ObservableCollection<Element>();
            Name = name;
            Results = new ObservableCollection<PRY>();
            property = new ObservableCollection<PRY>();
            property.Add(Tables.GetCoefficient(2));
            property.Add(Tables.GetCoefficient(3));
            property.Add(new Time());
            property.Add(new GammaPercent());
            property.Add(new Temperature());
        }


    }

    
}
