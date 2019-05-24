/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SqliteORM.ExpressionTree;

namespace SqliteORM
{
	public abstract class Where
	{
		public string Field { get; protected set; }

		internal string Build(SQLiteCommand command)
		{
			StringBuilder sql = new StringBuilder();
			sql.Append( Dialect.Keyword.Where );
			BuildImpl( sql, command );

			return sql.ToString();
		}

		internal abstract void BuildImpl(StringBuilder sql, SQLiteCommand command);
		

		protected Where()
		{}

		protected Where(string field)
		{
			Field = field;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
				return false;

			return ((Where) obj).Field == Field;
		}

		public static Where Expr<T>(Expression<Func<T, bool>> expr)
		{
			return (new WhereExpressionVisitor<T>()).ConvertToWhere(expr);
		}

		public static Where Or(params Where[] clauses)
		{
			return new ConcatinateWhere(Dialect.Keyword.Or, clauses);
		}

		public static Where And(params Where[] clauses)
		{
			return new ConcatinateWhere(Dialect.Keyword.And, clauses);
		}

        public static Where Equal(string field, object val)
        {
            if (val == null)
                return new WhereNull(field, false);
            return new WhereStringOp(field, val, "=");
        }

        public static Where Equal(TableColumn col, object val)
        {
            if (typeof(Enum).IsAssignableFrom(col.Type))
                val = (string)Enum.GetName(col.Type, val);

            return Equal(col.RawName, val);
        }
        
        public static Where NotEqual(TableColumn col, object val)
        {
            return NotEqual(col.RawName, val);
        }

        public static Where NotEqual(string field, object val)
        {
			if (val == null)
				return new WhereNull(field, true);

            return new WhereStringOp(field, val, "!=");
        }
        
        public static Where Like(TableColumn col, string val)
        {
            return Like(col.RawName, val);
        }

		public static Where Like(string field, string val)
		{
			return new WhereStringOp(field, val, " LIKE ");
		}

        public static Where GreaterThan(TableColumn col, object val)
        {
            return GreaterThan(col.RawName, val);
        }

        public static Where GreaterThan(string field, object val)
        {
            return new WhereStringOp(field, val, ">");
        }

        public static Where GreaterOrEqual(TableColumn col, object val)
        {
            return GreaterOrEqual(col.RawName, val);
        }

        public static Where GreaterOrEqual(string field, object val)
        {
            return new WhereStringOp(field, val, ">=");
        }

        public static Where LessThan(TableColumn col, object val)
        {
            return LessThan(col.RawName, val);
        }

        public static Where LessThan(string field, object val)
        {
            return new WhereStringOp(field, val, "<");
        }

        public static Where LessOrEqual(TableColumn col, object val)
        {
            return LessOrEqual(col.RawName, val);
        }

        public static Where LessOrEqual(string field, object val)
        {
            return new WhereStringOp(field, val, "<=");
        }

    }

	internal class ConcatinateWhere : Where
	{
		private readonly string _join;
		private readonly Where[] _clauses;

		internal ConcatinateWhere(string join, params Where[] clauses)
		{
			_join = join;
			_clauses = clauses;
		}

		internal override void BuildImpl(StringBuilder sql, SQLiteCommand command)
		{
			foreach (Where clause in _clauses)
			{
				clause.BuildImpl( sql, command );
				sql.AppendFormat(_join);
			}
			sql.Remove(sql.Length - _join.Length, _join.Length);
		}

		public override bool Equals(object obj)
		{
			if (!(typeof(ConcatinateWhere).IsAssignableFrom(obj.GetType())))
				return false;

			ConcatinateWhere objWhere = (ConcatinateWhere)obj;
			if (objWhere._join != _join)
				return false;

			if (objWhere._clauses.Length != _clauses.Length)
				return false;

			return !_clauses.Where( (t, i) => !objWhere._clauses[ i ].Equals( t ) ).Any();
		}

        public override int GetHashCode()
        {
            return unchecked( _join.GetHashCode() * _clauses.GetHashCode() );
        }
	}

	internal class WhereNull : Where
	{
		private readonly bool _isNotNull;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
				return false;

			return ((WhereNull)obj)._isNotNull == _isNotNull && base.Equals(obj);
		}

		public WhereNull(string field, bool isNotNull) : base(field)
		{
			_isNotNull = isNotNull;
		}

		internal override void BuildImpl(StringBuilder sql, SQLiteCommand command)
		{
			sql.AppendFormat("[{0}] is {1}null", Field, (_isNotNull ? "not " : ""));
		}
	}

	internal class WhereStringOp : Where
	{
		private readonly object _val;
        private readonly string _operation;

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
				return false;

			return _operation.Equals(((WhereStringOp)obj)._operation) && _val.Equals(((WhereStringOp)obj)._val) && base.Equals(obj);
		}

		internal WhereStringOp(string field, object val, string operation) : base(field)
		{
			_val = val;
            _operation = operation;
		}

        internal override void BuildImpl(StringBuilder sql, SQLiteCommand command)
		{
			string paramname = "@p_" + command.Parameters.Count;
			command.Parameters.Add( new SQLiteParameter( paramname, _val ) );
			sql.AppendFormat("{0} {1} {2}", Field, _operation, paramname);
		}
	}

    internal class SqlOperation : Where
    {
        private string _literal;

        internal SqlOperation( string literal )
        {
            _literal = literal;
        }

        internal override void BuildImpl( StringBuilder sql, SQLiteCommand command )
        {
            sql.Append( _literal );
        }
    }
}
