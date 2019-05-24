/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqliteORM
{
	public  class DbConnection : IDisposable
	{
        [ThreadStatic]
        private static SQLiteConnection _connection;

        [ThreadStatic]
        private static long _connectionOpenCount = 0;

        internal SQLiteConnection Connection 
        {
            get { return _connection ?? (_connection = new SQLiteConnection( _connectionString )); }
        }
        
		public static void Initialise(string connectionString)
		{
			Initialise( connectionString, System.Reflection.Assembly.GetCallingAssembly() );
		}

		public delegate void SqlListener(string sql);

		public static event SqlListener SqlListeners;

        internal static void BroadcastToListeners(string comment)
        {
            if (SqlListeners == null)
                return;

            SqlListeners(comment);
        }

		internal static void BroadcastToListeners(SQLiteCommand command)
		{
			if (SqlListeners == null)
				return;

			StringBuilder sql = new StringBuilder();
			sql.AppendFormat( command.CommandText );
			if (command.Parameters.Count > 0)
			{
				sql.AppendFormat( " | " );
				foreach (SQLiteParameter parm in command.Parameters)
					sql.AppendFormat( "{0}={1} ", parm.ParameterName, parm.Value );
			}

			SqlListeners( sql.ToString() );
		}

		public static void Initialise(string connectionString, Assembly asm)
		{
			_connectionString = connectionString;
            ReflectEntities( asm );
        }

        public static void ReflectEntities(Assembly asm)
        {
			foreach (var type in asm.GetTypes())
			{
				var attributes = type.GetCustomAttributes( typeof (TableAttribute), true );
				if (!attributes.Any())
					continue;

                TableMeta.AddType( type );               
			}
		}

		private static string _connectionString;
        
		public DbConnection()
		{
            long open;
            lock (typeof( DbConnection ))
            {
                open = DbConnection._connectionOpenCount;
                DbConnection._connectionOpenCount++;
            }

            if (open == 0)
            {
                BroadcastToListeners( "** Open connection" ); 
                Connection.Open();                
            }
        }

		public void Dispose()
		{
            long close;
            lock (typeof( DbConnection ))
            {
                DbConnection._connectionOpenCount--;
                close = DbConnection._connectionOpenCount;
            }

            if (close == 0)
            {
                Connection.Close();
                BroadcastToListeners( "** Close connection" );
            }
		}
                
        public static IEnumerable<string> VerifySchema()
        {
            var tableNames = TableNames(false);

            foreach (var meta in TableMeta.GetAll())
            {
                if (meta.TableParamCount > 0)
                    continue;

                if (!tableNames.Contains(meta.ParameterizedTableName))
                {
                    (new AnonymousAdapter(meta)).CreateTable();
                    meta.CheckExists = false;
                    yield return "Create table: " + meta.ParameterizedTableName;
                }
                else
                {
                    yield return "Exists table: " + meta.ParameterizedTableName;
                }
            }
            
        }

		public static IEnumerable<string> TableNames( bool excludeKnownTables )
		{
			using (SQLiteConnection conn = new SQLiteConnection(_connectionString))
			using (SQLiteCommand command = new SQLiteCommand( "SELECT name FROM sqlite_master WHERE type='table' AND Name not like 'sqlite_%';", conn ))
			{
				BroadcastToListeners( command );

				conn.Open();
				var reader = command.ExecuteReader();

				while (reader.Read())
					if ( !(excludeKnownTables && TableMeta.KnownTable( reader["name"] as string )) )
						yield return reader["name"] as string;

                reader.Dispose();
			}
		}
	}

}