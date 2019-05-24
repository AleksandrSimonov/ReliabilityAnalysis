/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqliteORM
{
	public class Query<T> : DbConnection, IEnumerable<T>
	{
		private string _sql;
		private TableMeta _meta;
		private Where _where;
		private List<OrderByClause> _orderBy = new List<OrderByClause>();
        private int _resultPage;
        private int _resultsPerPage;

        private List<T> _results = null;

        public Query()
        {
        }

		public Query( string sql, TableMeta meta )
		{
			_sql = sql;
			_meta = meta;
		}

		public Query<T> Where(Where clause)
		{
			if (_where != null)
				throw new InvalidOperationException("Can only have one Where expression");
			
			_where = clause;
			return this;
		}

		public Query<T> Where(Expression<Func<T,bool>> clause)
		{
			if (_where != null)
				throw new InvalidOperationException( "Can only have one Where expression" );

			_where = SqliteORM.Where.Expr<T>( clause );
			return this;
		}

        public Query<T> TakePage( int page, int resultsPerPage )
        {
            this._resultPage = page;
            this._resultsPerPage = resultsPerPage;
            return this;
        }

		public Query<T> OrderBy<TResult>(Expression<Func<T,TResult>> field)
		{
			return OrderBy( field, false );
		}

        public Query<T> OrderBy<TResult>(Expression<Func<T,TResult>> field, bool descending) 
		{
			if (field.NodeType == ExpressionType.Lambda && field.Body.NodeType == ExpressionType.MemberAccess)
			{
				var ex = ((MemberExpression) field.Body).Member;

				var colmn = _meta.Columns.First( col => col.FieldName == ex.Name );
				_orderBy.Add( new OrderByClause( colmn, descending ) );
			}

			return this;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator( )
		{
            if (_results == null)
            {
                _results = new List<T>();

                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    string clause = _where == null ? string.Empty : _where.Build(command);

                    command.CommandText = _sql + clause + BuildOrderBy() + BuildLimit();

                    BroadcastToListeners(command);

                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                        _results.Add((T)ReadRow(reader, _meta));

                    this.Dispose();
                }
            }

            foreach (T t in _results)
                yield return t;
		}

		private string BuildOrderBy( )
		{
			if (!_orderBy.Any())
				return string.Empty;

			StringBuilder orderBy = new StringBuilder();
			orderBy.Append( " ORDER BY" );
			_orderBy.ForEach( o => orderBy.AppendFormat( " {0}{1}", o.Column.Name, (o.Desending ? " DESC" : string.Empty) ));
			return orderBy.ToString();
		}

        private string BuildLimit()
        {
            if (_resultsPerPage == 0)
                return string.Empty;

            return string.Format( " LIMIT {0}, {1}", _resultsPerPage * _resultPage, _resultsPerPage );            
        }

		public virtual IEnumerator GetEnumerator( )
		{
			return ((IEnumerable<T>) this).GetEnumerator();
		}

		private object ReadRow(IDataRecord reader, TableMeta meta)
		{
            object instance = Activator.CreateInstance<T>();

			for (int i = 0; i < meta.Columns.Count; i++)
				if (Convert.IsDBNull(reader[i]))
					meta.Columns[i].SetValue(instance, null);
				else if (typeof(Enum).IsAssignableFrom(meta.Columns[i].Type))
					meta.Columns[i].SetValue(instance, Enum.Parse(meta.Columns[i].Type, (string)reader[i]));
				else
					meta.Columns[i].SetValue(instance, reader[i]);

			return instance;
		}

        internal void SetConnection( SQLiteConnection connection )
        {
            //Connection = connection;
        }
    }

	internal class OrderByClause
	{
		public OrderByClause(TableColumn column, bool descending)
		{
			Column = column;
			Desending = descending;
		}

		internal TableColumn Column { get; set; }
		internal bool Desending { get; set; }
	}
}