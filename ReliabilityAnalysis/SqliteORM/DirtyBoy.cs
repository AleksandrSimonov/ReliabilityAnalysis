using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Dynamic;

namespace SqliteORM
{
    public class DirectSql : DbConnection
    {
        public void Execute(string sql, Func<object[], bool> perRow)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                object[] vals = new object[reader.VisibleFieldCount];

                while (reader.Read())
                {
                    reader.GetValues(vals);
                    if (!perRow(vals))
                        break; 
                }

                reader.Dispose();
            }
        }

        public IEnumerable<dynamic> ExecuteDynamic(string sql, List<string> parameters = null)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                if (parameters != null || parameters.Count() > 0)
                    for (int index = 0; index < parameters.Count(); index++)
                        command.Parameters.Add( new SQLiteParameter( string.Format( "p{0}", index ), parameters[index] ) );
                
                SQLiteDataReader reader = command.ExecuteReader();
                
                int fieldCount = reader.VisibleFieldCount;

                while (reader.Read())
                {
                    dynamic expando = new ExpandoObject();
                    for (int idx = 0; idx < fieldCount; idx++)
                        ((IDictionary<string, object>)expando).Add(new KeyValuePair<string, object>(reader.GetName(idx), reader[idx]));

                    yield return expando;
                }

                reader.Dispose();
            }
        }        
    }
}
