using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqliteORM;
using System.Collections.ObjectModel;
using Ciloci.Flee;
namespace ReliabilityAnalysis.DataBase
{
    public static class Tables
    {
        public static string StringConnection { get; set; }
        [Table]
        public abstract class IdElement
        {
            [PrimaryKey] public Int64 ID { get; set; }
            [Field] public string Name { get; set; }
        }
        [Table]
        public class Class : IdElement
        {
            public static string GetName(Int64 ID)
            {
                DbConnection.Initialise(StringConnection);
                /* List<Class> row;
                 using (var name = TableAdapter<Class>.Open())
                     row = name.Select().Where(tbl => tbl.ID == Convert.ToInt64(ID)).ToList();
                 if (row == null)
                     return null;
                 else
                     return row[0].Name;*/
                using (var name = TableAdapter<Type>.Open())
                    return name.Select().FirstOrDefault(tbl => tbl.ID == Convert.ToInt64(ID)).Name;
            }
            public static List<Node> GetItems()
            {
                DbConnection.Initialise(StringConnection);
                var nodes = new List<Node>();

                using (var adapter = TableAdapter<Class>.Open())
                {

                    foreach (var rows in adapter.Select())
                    {
                        var row = (Class)rows;
                        nodes.Add(new Node() { ID_Class = row.ID, Name = row.Name, DefDesign = row.DefDesign });
                    }


                }

                return nodes;

            }
            [Field] public string DefDesign { get; set; }
        }
        [Table]
        public class Scroll : IdElement
        {
            [ForeignKey(typeof(Class))] public Int64 ID_Class { get; set; }
            public static string GetName(Int64 ID)
            {
                DbConnection.Initialise(StringConnection);
                /* List<Scroll> row;
                 using (var name = TableAdapter<Scroll>.Open())
                     row = name.Select().Where(tbl => tbl.ID == Convert.ToInt64(ID)).ToList();
                 if (row == null)
                     return null;
                 else
                     return row[0].Name;*/
                using (var name = TableAdapter<Scroll>.Open())
                    return name.Select().FirstOrDefault(tbl => tbl.ID == Convert.ToInt64(ID)).Name;
            }
            public static ObservableCollection<Node> GetItems(int ID_Class)
            {
                DbConnection.Initialise(StringConnection);
                var nodes = new ObservableCollection<Node>();

                using (var adapter = TableAdapter<Scroll>.Open())
                {

                    foreach (var rows in adapter.Select().Where(tbl => tbl.ID_Class == Convert.ToInt64(ID_Class)))
                    {
                        var row = (Scroll)rows;
                        nodes.Add(new Node() { ID_Class = ID_Class, ID_Scroll = row.ID, Name = row.Name });
                    }


                }

                return nodes;

            }

        }
        [Table]
        public class Type : IdElement
        {
            [ForeignKey(typeof(Scroll))] public Int64 ID_Scroll { get; set; }
            public static string GetName(Int64 ID)
            {
                DbConnection.Initialise(StringConnection);
                /* List<Type> row;
                 using (var name = TableAdapter<Type>.Open())
                     row = name.Select().Where(tbl => tbl.ID == Convert.ToInt64(ID)).ToList();
                 if (row.Count == 0)
                     return null;

                 return row[0].Name;*/
                try
                {
                    using (var name = TableAdapter<Type>.Open())
                        return name.Select().FirstOrDefault(tbl => tbl.ID == Convert.ToInt64(ID)).Name;
                }
                catch (Exception)
                {
                    return null;
                }

            }
            public static ObservableCollection<Node> GetItems(int ID_Class, int ID_Scroll)
            {
                DbConnection.Initialise(StringConnection);
                var nodes = new ObservableCollection<Node>();

                using (var adapter = TableAdapter<Type>.Open())
                {

                    foreach (var rows in adapter.Select().Where(tbl => tbl.ID_Scroll == Convert.ToInt64(ID_Scroll)))
                    {
                        var row = (Type)rows;
                        nodes.Add(new Node() { ID_Class = ID_Class, ID_Scroll = ID_Scroll, ID_Type = row.ID, Name = row.Name });
                    }


                }

                return nodes;

            }
        }
        [Table]
        [Serializable]
        public class Info
        {
            [PrimaryKey] public Int64 ID { get; set; }
            [Field] public string Discription { get; set; }
            [ForeignKey(typeof(KIndex))] public Int64 ID_KIndex { get; set; }
            public override string ToString()
            {
                return Discription;
            }
            public override int GetHashCode()
            {
                return Discription.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                return Discription.Equals(((Info)obj).Discription);
            }
        }
        [Table]
        public class MathModels
        {
            [PrimaryKey] public Int64 ID { set; get; }
            [Field] public string MathModel { set; get; }
            [ForeignKey(typeof(Type))] public Int64 ID_Type { set; get; }
            [ForeignKey(typeof(Scroll))] public Int64 ID_Scroll { set; get; }
            [ForeignKey(typeof(Class))] public Int64 ID_Class { set; get; }

        }
        [Table]
        public class Kmodel
        {
            [PrimaryKey] public Int64 ID { get; set; }
            [Field] public double A { get; set; }
            [Field] public double B { get; set; }
            [Field] public double Nt { get; set; }
            [Field] public double G { get; set; }
            [Field] public double Ns { get; set; }
            [Field] public double J { get; set; }
            [Field] public double H { get; set; }
            [ForeignKey(typeof(Type))] public Int64 ID_Type { get; set; }
            [ForeignKey(typeof(Scroll))] public Int64 ID_Scroll { get; set; }
            [ForeignKey(typeof(Class))] public Int64 ID_Class { get; set; }
            [ForeignKey(typeof(KIndex))] public Int64 ID_KIndex { get; set; }
        }
        [Table]
        public class KIndex : IdElement
        {
            [Field] public string Discription { get; set; }
            [Field] public string ParamName { get; set; }
            [Field] public string ParamDiscription { get; set; }
        }
        [Table]
        public class Coefficient
        {
            [PrimaryKey] public Int64 ID { get; set; }
            [ForeignKey(typeof(Info))] public Int64 Info { get; set; }
            [Field] public double ParamMin { get; set; }
            [Field] public double ParamMax { get; set; }
            [Field] public double ParamFix { get; set; }
            [Field] public string MathModel { set; get; }
            [Field] public double Value { get; set; }
            [ForeignKey(typeof(Type))] public Int64 ID_Type { get; set; }
            [ForeignKey(typeof(Scroll))] public Int64 ID_Scroll { get; set; }
            [ForeignKey(typeof(Class))] public Int64 ID_Class { get; set; }
            [ForeignKey(typeof(KIndex))] public Int64 ID_KIndex { get; set; }


        }
        [Table]
        public class Lambda
        {
            [PrimaryKey] public Int64 ID { get; set; }
            [Field] public double Value { get; set; }
            [ForeignKey(typeof(Type))] public Int64 ID_Type { get; set; }
            [ForeignKey(typeof(Scroll))] public Int64 ID_Scroll { get; set; }
            public static double GetLambdaValue(Node item)
            {
                DbConnection.Initialise(StringConnection);
                Int64 ID = item.ID_Type;
                string col = "ID_Type";
                {
                    if (ID == 0)
                        if (item.ID_Scroll == 0)
                        {
                            ID = item.ID_Class;
                            col = "ID_Class";
                        }
                        else
                        {
                            ID = item.ID_Scroll;
                            col = "ID_Scroll";
                        }
                }

                using (var value = TableAdapter<Lambda>.Open())
                    return value.Select().Where(Where.Equal(col, Convert.ToInt64(ID))).ToList()[0].Value * Math.Pow(10, -6);
            }
        }
        public static ObservableCollection<Node> ShowBD()
        {
            DbConnection.Initialise(StringConnection);
            var nodes = new ObservableCollection<Node>(Class.GetItems());

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Nodes = Scroll.GetItems((int)nodes[i].ID_Class);

                for (int j = 0; j < nodes[i].Nodes.Count; j++)
                {
                    nodes[i].Nodes[j].Nodes = Type.GetItems((int)nodes[i].ID_Class, (int)nodes[i].Nodes[j].ID_Scroll);
                }

            }
            return nodes;
        }
        public static double GetCoefficientValue(Node item, K coeff)
        {
            DbConnection.Initialise(StringConnection);

            Int64 infoId = 0;
            double value = 0;

            List<Coefficient> rows;


            if (Double.TryParse(coeff.SelectedParamValue, out value) == false)
                using (var adapter = TableAdapter<Info>.Open())
                    infoId = adapter.Select().First(tbl => (tbl.ID_KIndex == coeff.ID_KIndex) && (tbl.Discription == coeff.SelectedParamValue)).ID;


            using (var adapter = TableAdapter<Coefficient>.Open())
                rows = adapter.Select().Where(tbl => (tbl.ID_KIndex == coeff.ID_KIndex) && ((tbl.ID_Type == item.ID_Type) || (
                                                            (tbl.ID_KIndex == coeff.ID_KIndex) && (tbl.ID_Type == null) && (tbl.ID_Scroll == item.ID_Scroll)) || (
                                                            (tbl.ID_KIndex == coeff.ID_KIndex) && (tbl.ID_Type == null) && (tbl.ID_Scroll == null) && (tbl.ID_Class == item.ID_Class))
                                                             )).ToList();

            if (infoId != 0)
            {
                foreach (var row in rows)
                    if (row.Info == infoId)
                        return row.Value;
            }

            if (rows.FirstOrDefault(r => r.MathModel != null) != null)
            {
                Kmodel m;
                using (var adapter = TableAdapter<Kmodel>.Open())
                    m = adapter.Select().FirstOrDefault(tbl => (tbl.ID_KIndex == coeff.ID_KIndex) && ((tbl.ID_Type == item.ID_Type) || (
                                                                  (tbl.ID_KIndex == coeff.ID_KIndex) && (tbl.ID_Type == null) && (tbl.ID_Scroll == item.ID_Scroll)) || (
                                                                  (tbl.ID_KIndex == coeff.ID_KIndex) && (tbl.ID_Type == null) && (tbl.ID_Scroll == null) && (tbl.ID_Class == item.ID_Class))
                                                                   ));

                if (m == null)
                    m = new Kmodel();

                ExpressionContext context = new ExpressionContext();
                context.Imports.AddType(typeof(Math));
                List<string> s = context.Variables.Keys.ToList();//

                context.Variables["A"] = m.A; context.Variables["Ns"] = m.Ns;
                context.Variables["B"] = m.B; context.Variables["Nt"] = m.Nt;
                context.Variables["G"] = m.G; context.Variables["t"] = 30.1;
                context.Variables["t"] = 30.1;
                context.Variables["H"] = m.H; context.Variables["value"] = value;
                context.Variables["J"] = m.J;

                IDynamicExpression eDynamic = context.CompileDynamic(rows[0].MathModel);
                var x = (double)eDynamic.Evaluate();
                return Math.Round((double)eDynamic.Evaluate(), 4);
            }
            if (rows.FirstOrDefault(r => (r.ParamMin != 0) || (r.ParamMax != 0)) != null)
                foreach (var row in rows)
                    if ((row.ParamMin <= value) && (value < row.ParamMax))
                        return row.Value;

            if (rows.FirstOrDefault(r => r.ParamFix != 0) != null)
                foreach (var row in rows)
                    if (row.ParamFix != value)
                        return row.Value;
            return value;
        }
        public static ObservableCollection<K> GetCoefficients(Node item)
        {
            DbConnection.Initialise(StringConnection);
            ObservableCollection<K> k;

            using (var adapter = TableAdapter<Coefficient>.Open())
            {
                k = new ObservableCollection<K>();

                var rows = adapter.Select().Where(tbl => (tbl.ID_Type == item.ID_Type) || (
                                                          (tbl.ID_Type == null) && (tbl.ID_Scroll == item.ID_Scroll)) || (
                                                          (tbl.ID_Type == null) && (tbl.ID_Scroll == null) && (tbl.ID_Class == item.ID_Class))).ToList();

                for (int i = 0; i < rows.Count; i++)
                    if ((rows[i].ID_KIndex != 2) && (rows[i].ID_KIndex != 3))
                        k.Add(new K(rows[i].ID_KIndex));

                k = new ObservableCollection<K>(k.Distinct());
            }

            using (var adapter = TableAdapter<Info>.Open())
                for (int i = 0; i < k.Count; i++)
                    k[i].Info = adapter.Select().Where(tbl => tbl.ID_KIndex == Convert.ToInt64(k[i].ID_KIndex)).ToList();

            using (var adapter = TableAdapter<KIndex>.Open())
                for (int i = 0; i < k.Count; i++)
                {
                    var row = adapter.Select().First(tbl => tbl.ID == Convert.ToInt64(k[i].ID_KIndex));
                    k[i].Name = row.Name;
                    k[i].ParamName = row.ParamName;
                    k[i].ParamDiscription = row.ParamDiscription;
                    k[i].Discription = row.Discription;
                    k[i].ID = item;
                }

            return k;

        }
        public static K GetCoefficient(long idOfIndex)
        {
            DbConnection.Initialise(StringConnection);
            var k = new K();

            using (var adapter = TableAdapter<Info>.Open())
                k.Info = adapter.Select().Where(tbl => tbl.ID_KIndex == Convert.ToInt64(idOfIndex)).ToList();

            using (var adapter = TableAdapter<KIndex>.Open())
            {
                var row = adapter.Select().First(tbl => tbl.ID == Convert.ToInt64(idOfIndex));
                k.Name = row.Name;
                k.ParamName = row.ParamName;
                k.ParamDiscription = row.ParamDiscription;
                k.Discription = row.Discription;
                k.ID_KIndex = idOfIndex;
            }

            return k;
        }
        public static string GetMathModel(Node item)
        {
            DbConnection.Initialise(StringConnection);
            List<MathModels> MathModel = null;

            using (var rows = TableAdapter<MathModels>.Open())
            {
                MathModel = rows.Select().Where(tbl => (tbl.ID_Class == item.ID_Class) &&
                                                    (tbl.ID_Scroll == item.ID_Scroll) &&
                                                    (tbl.ID_Type == item.ID_Type)).ToList();

                if (MathModel.Count == 0)
                    MathModel = rows.Select().Where(tbl => (tbl.ID_Class == item.ID_Class) &&
                                                    (tbl.ID_Scroll == item.ID_Scroll)).ToList();

            }

            return MathModel[0].MathModel;

        }
        public static string GetDefDesign(Node item)
        {
            DbConnection.Initialise(StringConnection);

            using (var rows = TableAdapter<Class>.Open())
            {
                return rows.Select().First(tbl => (tbl.ID == item.ID_Class)).DefDesign;
            }

        }
    }
}
