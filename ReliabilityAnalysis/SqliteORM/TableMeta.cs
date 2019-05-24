/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqliteORM.Dialect;

namespace SqliteORM
{
	public class TableMetaInvalid : TableMeta
	{
		public List<String> Reasons = new List<string>();
	}

	public class TableMeta
	{
		public string ParameterizedTableName;
		public int TableParamCount;
        public List<TableColumn> Columns = new List<TableColumn>();
		public bool HasAutoIncrementPrimaryKey;
		public bool CheckExists = true;
        private Type _tableType;
        public Type TableType { get { return _tableType; } }

        private static readonly Dictionary<Type, TableMeta> TableMetaDictionaryType = new Dictionary<Type, TableMeta>();
		private static readonly Dictionary<string, TableMeta> TableMetaDictionaryString = new Dictionary<string, TableMeta>();

		internal static bool KnownTable( string tableName )
		{
			return (TableMetaDictionaryType.Any( 
				meta =>

					{
						string name = meta.Value.ParameterizedTableName;
						string compare = tableName;
						int cutoff = name.IndexOf( '{' );
						if (cutoff != -1)
						{
							if (cutoff > tableName.Length)
								return false;

							name = name.Substring( 0, cutoff );
							compare = compare.Substring( 0, cutoff );
						}
						
						return string.Equals( name, compare, StringComparison.OrdinalIgnoreCase );
					}
				));
		}

        internal static TableMeta Get(Type t)
        {
        	TableMeta meta;
			if (TableMetaDictionaryType.TryGetValue(t, out meta))
				return meta;

        	return new TableMetaInvalid() {Reasons = new List<string>() {"Unknown type " + t.FullName + ". Check it has [Table] attribute"}};
        }

		internal static TableMeta Get(string tableName)
		{
			TableMeta meta;
			if (!TableMetaDictionaryString.TryGetValue(tableName, out meta))
			{
				meta = SchemaParse.ToMeta(tableName);
				if (meta == null)
					return new TableMetaInvalid() { Reasons = new List<string>() { "Unknown table" } };

				TableMetaDictionaryString.Add( tableName, meta );
				meta.HasAutoIncrementPrimaryKey = meta.Columns.Any(col => col.PrimaryKey && col.AutoIncrement);
			}

			return meta;
		}

        public string CreateTableName(object[] args)
        {
            return string.Format(ParameterizedTableName, args);
        }

		internal static void AddType(Type type )
		{
			TableMeta meta;

            if (TableMetaDictionaryType.TryGetValue(type, out meta))
            {
                meta.CheckExists = true;
                return; 
            }

            meta = new TableMeta() { _tableType = type };
           
			GetTableName( meta, type );

			var errors = CreateColumnList( meta, type );
			if (errors.Any())
				meta = new TableMetaInvalid() {Reasons = errors.ToList()};

			if (meta.Columns.Count == 0 && !(meta is TableMetaInvalid))
				meta = new TableMetaInvalid();

			if (meta.Columns.Count == 0)
				((TableMetaInvalid)meta).Reasons.Add("No columns defined");
			else
				meta.HasAutoIncrementPrimaryKey = meta.Columns.Any( col => col.PrimaryKey && col.AutoIncrement );

            DbConnection.BroadcastToListeners("Add new type: " + type.FullName);
			TableMetaDictionaryType.Add( type, meta );
		}

		private static void GetTableName( TableMeta meta, Type type )
		{
			var attributes = type.GetCustomAttributes( typeof( TableAttribute ), true );
			meta.ParameterizedTableName = string.IsNullOrEmpty( ((TableAttribute)attributes[0]).Name )
								? type.Name
								: ((TableAttribute)attributes[0]).Name;

			meta.TableParamCount = meta.ParameterizedTableName.Count( c => c == '{' );
		}


		private static IEnumerable<string> CreateColumnList( TableMeta meta, Type type )
		{
            List<PropertyFieldInfo> members = new List<PropertyFieldInfo>();
            
            foreach (PropertyInfo pi in type.GetProperties( BindingFlags.Public | BindingFlags.Instance ))
			    members.Add(new PropertyFieldInfo(pi));

            foreach (FieldInfo fi in type.GetFields( BindingFlags.Public | BindingFlags.Instance ))
			    members.Add(new PropertyFieldInfo(fi));

            foreach (var member in members)
			{
				var definition = member.FieldAttribute();
                if (definition == null)
                    continue;

				if (!Column.IsSupported(member.Type))
				{
					yield return "Unsupported type " + member.Type + " in field " + member.Name;
					continue;
				}

				if (string.IsNullOrEmpty( definition.Name ))
					definition.Name = member.Name;

                meta.Columns.Add(new TableColumn(definition, member));
 
			}

			yield break;
		}


        internal static IEnumerable<TableMeta> GetAll()
        {
            return from t in TableMetaDictionaryType select t.Value;
        }
    }
}