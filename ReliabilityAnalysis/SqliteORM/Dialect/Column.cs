/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace SqliteORM.Dialect
{
	public class Column
	{
		public const string Nullable = "";
		public const string NotNullable = "not null";
		public const string PrimaryKey = "PRIMARY KEY ";
		public const string OnConflictReplace = "ON CONFLICT REPLACE";

		private class TypeConvert
		{
			public DbType DbType;
			public string SqlType;

			public TypeConvert(DbType dbType, string sqlType)
			{
				DbType = dbType;
				SqlType = sqlType;
			}
		}

        public static string CreateFieldDefinition( Type type, bool primaryKey, bool isNullable, bool autoIncrement, bool isCaseInsensitive )
		{
			if (autoIncrement && (type == typeof( long ) || type == typeof(int)) && primaryKey)
				return "integer primary key autoincrement";

            if (autoIncrement && (type == typeof(long) || type == typeof(int)))
                return "integer autoincrement";

            return string.Format("{0} {1}",
			                     ToSqlType(type, isCaseInsensitive),
			                     isNullable ? Nullable : NotNullable);
		}

		private static List<KeyValuePair<Type, TypeConvert>> convertList = new List<KeyValuePair<Type, TypeConvert>>()
		                                                                   	{
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Boolean), new TypeConvert(DbType.Boolean, "bit")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Char), new TypeConvert(DbType.String, "char")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(SByte), new TypeConvert(DbType.SByte, "int")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Byte), new TypeConvert(DbType.Byte, "byte")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Int16), new TypeConvert(DbType.Int16, "smallint")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(UInt16), new TypeConvert(DbType.UInt16, "int")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Int32), new TypeConvert(DbType.Int32, "int")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Int64), new TypeConvert(DbType.Int64, "long")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Single), new TypeConvert(DbType.Single, "real")),
                                                                                new KeyValuePair<Type, TypeConvert>(typeof(float), new TypeConvert(DbType.Single, "real")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Double), new TypeConvert(DbType.Double, "double")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Decimal), new TypeConvert(DbType.Decimal, "double")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(DateTime), new TypeConvert(DbType.DateTime, "date")),
                                                                                new KeyValuePair<Type, TypeConvert>(typeof(TimeSpan), new TypeConvert(DbType.Object, "object")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(Guid), new TypeConvert(DbType.Guid, "guid")),
		                                                                   		new KeyValuePair<Type, TypeConvert>(typeof(String), new TypeConvert(DbType.String, "ntext"))
		                                                                   	};

		public static bool IsSupported(Type type)
		{
			if (typeof( Enum ).IsAssignableFrom( type ))
				return true;

			return convertList.Any( t => t.Key == type );
		}

		public static DbType ToDbType(Type type)
		{
			if (typeof( Enum ).IsAssignableFrom( type ))
				return DbType.String;

			return convertList.First(t => t.Key == type).Value.DbType;
		}

        private static string ToSqlType( Type type, bool isCaseInsensitive )
		{
            if (isCaseInsensitive && type == typeof( string ))
                return "varchar(100) collate nocase";

			if (typeof( Enum ).IsAssignableFrom( type ))
				return "text";
            
			return convertList.First( t => t.Key == type ).Value.SqlType;
		}

		public static string SafeFieldName(string name)
		{
			return "[" + name + "]";
		}

		public static string ParamName(string name)
		{
			return "@" + name;
		}
	}
}