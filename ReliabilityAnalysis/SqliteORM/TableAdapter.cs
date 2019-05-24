/*
 *  Sqlite ORM - GNU General Public License, version 3 (GPL-3.0)
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */

using System;
using System.Data.SQLite;
using SqliteORM.Dialect;

namespace SqliteORM
{
    
	public class TableAdapter<T> : DbConnection 
	{
		protected TableMeta _meta;
		protected string _tableName;
        private bool _isMultiTableJoin = false;

		public static TableAdapter<T> Open(params object[] args)
		{
            TableAdapter<T> adapter = Activator.CreateInstance<TableAdapter<T>>();

            if (typeof(IMash).IsAssignableFrom(typeof(T)))
            {
                adapter._isMultiTableJoin = true;
                return adapter;
            }

            TableMeta meta = TableMeta.Get(typeof(T));
			if (meta is TableMetaInvalid)
				throw new ArgumentException( typeof (T).FullName + " is invalid because " + string.Join( " and ", ((TableMetaInvalid) meta).Reasons.ToArray() ) );

			if (args.Length != meta.TableParamCount)
				throw new ArgumentException("Please specify " + meta.TableParamCount + " parameters");

            string tableName = meta.CreateTableName(args);

			if (meta.CheckExists)
			{
				adapter.CreateTable( meta, tableName );
				// not a dynamicly created table, so don't need to check it exists now it's created
				meta.CheckExists = (meta.TableParamCount > 0);
			}
                       
			adapter._meta = meta;
			adapter._tableName = tableName;

			return adapter;
		}

        public bool AreSame(T one, T two)
        {
            foreach (var tableColumn in _meta.Columns)
            {
                if (tableColumn.IsCaseInsensitive && string.Compare((string)tableColumn.GetValue(one), (string)tableColumn.GetValue(two), true) != 0)
                {
                    return false;
                }
                else
                {
                    if (tableColumn.IsFloatingPoint &&
                        Math.Abs( Convert.ToDouble( tableColumn.GetValue( one ) ) -
                                  Convert.ToDouble( tableColumn.GetValue( two ) ) ) > 9E+10)
                        return false;
                    else
                    {
                        if (!tableColumn.GetValue( one ).Equals( tableColumn.GetValue( two ) ))
                            return false;
                    }
                }
            }

			return true;
        }
        
		private string _updateSql;
		private string _selectRowSql;
		private string _selectAllSql;
		private string _deleteRowSql;
		private string _deleteAllSql;
		
		private string UpdateSql { get { return _updateSql ?? (_updateSql = Actions.UpdateSql(_meta, _tableName)); } }
		private string SelectRowSql { get { return _selectRowSql ?? (_selectRowSql = Actions.SelectRowSql(_meta, _tableName)); } }
		private string SelectAllSql { get { return _selectAllSql ?? (_selectAllSql = Actions.SelectAllSql(_meta, _tableName)); } }
		private string DeleteRowSql { get { return _deleteRowSql ?? (_deleteRowSql = Actions.DeleteRowSql(_meta, _tableName)); } }
		private string DeleteAllSql { get { return _deleteAllSql ?? (_deleteAllSql = Actions.DeleteSql(_meta, _tableName)); } }

		private void CreateTable(TableMeta tableMeta, string tableName)
		{
			ExecuteSql(Actions.CreateTable(tableMeta, tableName));
		}
        
		public long CreateUpdate(params object[] args)
		{
            if (_isMultiTableJoin)
                throw new NotSupportedException();

			long id;
			if (args.Length == 1 && (typeof(T).IsAssignableFrom(args[0].GetType())))
			{
				id = ExecuteUpdate((T)args[0]);

				if (_meta.HasAutoIncrementPrimaryKey)
					_meta.Columns.ForEach(col => { if (col.PrimaryKey && col.AutoIncrement) col.SetValue(args[0], id); });
			}
			else
			{
				id = ExecuteInsert( args );
			}
			return id;
		}
        
		public void Delete(params object[] args)
		{
            if (_isMultiTableJoin)
                throw new NotSupportedException();

			if (args.Length == 1 && (typeof(T).IsAssignableFrom(args[0].GetType())))
				ExecuteDeleteInst((T)args[0]);
			else if (args.Length == 1 && (typeof(Where).IsAssignableFrom(args[0].GetType())))
				ExecuteDeleteWhere((Where)args[0]);
			else
				ExecuteDelete(args);
		}
        
		public void DeleteAll()
		{
            if (_isMultiTableJoin)
                throw new NotSupportedException();

			ExecuteSql(DeleteAllSql);
		}

		public T Read(params object[] args)
		{
            if (_isMultiTableJoin)
                throw new NotSupportedException();

            return ExecuteRow(args);
		}

		public Query<T> Select()
		{
            Query<T> query;

            if (_isMultiTableJoin)
            {
                IMash t = Activator.CreateInstance<T>() as IMash;
                if (t == null)
                    throw new Exception("puke");

                query = new Query<T>( t.SelectAllSql(), (TableMeta)t );
            }
            else
            {
                query =new Query<T>(SelectAllSql, _meta);
            }

            //query.SetConnection( Connection );

            return query;
		}

		private void QueryParams(SQLiteCommand command, bool insertOp, params object[] args)
		{
            if (_isMultiTableJoin)
                throw new NotSupportedException();

            if (args.Length > _meta.Columns.Count)
				throw new Exception("Too many parameters");

            int argIndex = 0;
            for (int paramIndex = 0; paramIndex < _meta.Columns.Count && argIndex < args.Length; paramIndex++)
            {
                if (_meta.Columns[paramIndex].AutoIncrement && insertOp)
                {
                    command.Parameters.Add(_meta.Columns[paramIndex].ParamName, _meta.Columns[paramIndex].DbType).Value = null;
                }
                else
                {
                    command.Parameters.Add(_meta.Columns[paramIndex].ParamName, _meta.Columns[paramIndex].DbType).Value = args[argIndex];
                    argIndex++;
                }
            }
		}

		private long ExecuteUpdate(T instance)
		{
			using (SQLiteCommand command = new SQLiteCommand(UpdateSql + " select last_insert_rowid();", Connection))
			{
				for (int i = 0; i < _meta.Columns.Count; i++)
					if (_meta.Columns[i].PrimaryKey &&  _meta.Columns[i].IsDefaultValue(instance))
						command.Parameters.Add( _meta.Columns[ i ].ParamName, _meta.Columns[ i ].DbType ).Value = null;
					else
						command.Parameters.Add(_meta.Columns[i].ParamName, _meta.Columns[i].DbType).Value = _meta.Columns[i].GetValue(instance);

				BroadcastToListeners( command );

				return (long)command.ExecuteScalar();
			}
		}

		private long ExecuteInsert(params object[] args)
		{
			using (SQLiteCommand command = new SQLiteCommand(UpdateSql + " select last_insert_rowid();", Connection))
			{
				QueryParams( command, true, args );
				
				BroadcastToListeners( command );

				return (long)command.ExecuteScalar();
			}
		}

		private T ExecuteRow(params object[] args)
		{
			T instance = Activator.CreateInstance<T>();

			using (SQLiteCommand command = new SQLiteCommand(SelectRowSql, Connection))
			{
				QueryParams(command, false, args);

				BroadcastToListeners( command );

				SQLiteDataReader reader = command.ExecuteReader();
				if (!reader.Read())
					return default(T);

				for (int i = 0; i < _meta.Columns.Count; i++)
				{
					object fieldValue = ConvertType(reader[ i ], _meta.Columns[i].Type, reader[i].GetType());
					_meta.Columns[i].SetValue(instance, fieldValue);
				}
			}

			return instance;
		}

		private object ConvertType(object val, Type to, Type from)
		{
			if (from.IsAssignableFrom(to))
				return Convert.ChangeType( val, to );

			if (to == typeof(TimeSpan))
				return TimeSpan.Parse( val.ToString() );

			if (to == typeof(UInt16))
				return Convert.ToUInt16(val);

			if (to == typeof(Decimal))
			{
				if (((double)val) >= (double)Decimal.MaxValue)
					return Decimal.MaxValue;

				if (((double)val) <= (double)Decimal.MinValue)
					return Decimal.MinValue;

				return Convert.ToDecimal( val );
			}

            if (to == typeof( Single ) && from == typeof( Double ))
                return Convert.ToSingle( val );

            if (typeof(Enum).IsAssignableFrom(to))
            {
                try
                {
                    return Enum.Parse(to, val as string);
                }
                catch
                {
                    return null;
                }
            }

			return null;
		}

		private void ExecuteDelete(params object[] args)
		{
			using (SQLiteCommand command = new SQLiteCommand(DeleteRowSql, Connection))
			{
				QueryParams(command, false, args);
				BroadcastToListeners( command );

				command.ExecuteNonQuery();
			}
		}

		private void ExecuteDeleteInst(T instance)
		{
			using (SQLiteCommand command = new SQLiteCommand(DeleteRowSql, Connection))
			{
				for (int i = 0; i < _meta.Columns.Count; i++)
				{
					if (!_meta.Columns[ i ].PrimaryKey)
						continue;

					command.Parameters.Add( _meta.Columns[ i ].ParamName, _meta.Columns[ i ].DbType ).Value = _meta.Columns[ i ].GetValue( instance);
				}

				BroadcastToListeners( command );
				command.ExecuteNonQuery();
			}
		}

        private void ExecuteDeleteWhere(Where where)
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.Connection = Connection;
                command.CommandText = DeleteAllSql + where.Build(command);
				BroadcastToListeners( command );

                command.ExecuteNonQuery();
            }
        }
        
		protected void ExecuteSql(string sql)
		{
			using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
			{
				BroadcastToListeners( command );
				command.ExecuteNonQuery();
			}
		}
	}
}