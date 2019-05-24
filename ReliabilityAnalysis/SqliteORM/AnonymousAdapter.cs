/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System.Collections.Generic;
using SqliteORM.Dialect;

namespace SqliteORM
{
	public sealed class AnonmousTable 
	{
		public AnonmousTable()
		{ }

        private Dictionary<string, object> values = new Dictionary<string, object>();

		public object this[string field]
		{
			get { return values[field]; }
            set { values[field] = value; }
		}
	}

	public class AnonymousAdapter : TableAdapter<AnonmousTable>
	{
		public TableMeta Meta { get { return _meta;  } }

		public static AnonymousAdapter Open(string tableName)
		{
			TableMeta meta = TableMeta.Get( tableName );
			if (meta == null)
				return null;

			return new AnonymousAdapter(meta)
				  {					  
					  _tableName = meta.ParameterizedTableName
				  };
		}

        public AnonymousAdapter(TableMeta meta)
        {
            _meta = meta;
        }

        public void CreateTable(params object[] args)
        {
            ExecuteSql(Actions.CreateTable(_meta, _meta.CreateTableName(args)));
        }
	}
}
