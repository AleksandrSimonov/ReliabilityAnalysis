/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SqliteORM.ExpressionTree
{
	public class WhereExpressionVisitor<T> : ExpressionVisitor
	{
		private readonly WhereBuilder _wherebuilder = new WhereBuilder();

		private TableMeta _meta;

		public Where ConvertToWhere(Expression expression)
		{
			_meta = TableMeta.Get( typeof (T) );
			Visit( expression );
			return _wherebuilder.Current.ToWhere();
		}

		protected override Expression VisitMemberAccess(MemberExpression memberExpression)
        {   
            // Property which returns a ValueType
            if (memberExpression.Member is PropertyInfo && typeof(ValueType).IsAssignableFrom(((PropertyInfo)memberExpression.Member).PropertyType))
            {
                PropertyInfo propInfo = (PropertyInfo)memberExpression.Member;                
                if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                {
                    MemberExpression innerMember = (MemberExpression)memberExpression.Expression;
                    return VisitConstant(Expression.Constant(propInfo.GetValue(((FieldInfo)innerMember.Member).GetValue(((ConstantExpression)innerMember.Expression).Value), null)));
                }                
            }

            // Field which returns a ValueType
            if (memberExpression.Member is FieldInfo && typeof(ValueType).IsAssignableFrom(((FieldInfo)memberExpression.Member).FieldType))
            {                                
                FieldInfo fieldInfo = (FieldInfo)memberExpression.Member;
                if (fieldInfo.IsStatic)
                {
                    // Static Field
                    return VisitConstant(Expression.Constant(fieldInfo.GetValue(null)));
                }
                else
                {
                    // Constant  [i.e. public const int MyValue]
                    if (memberExpression.Expression is ConstantExpression)
                    {
                        return Visit(memberExpression.Expression);
                    }
                    // Normal field type
                    else if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                    {
                        MemberExpression memberExpr = (MemberExpression)memberExpression.Expression;
                        return VisitConstant(Expression.Constant(fieldInfo.GetValue(((FieldInfo)memberExpr.Member).GetValue(((ConstantExpression)memberExpr.Expression).Value))));
                    }
                }
            }

        	var col = _meta.Columns.FirstOrDefault( c => c.FieldName == memberExpression.Member.Name );
            if (col == null)
                return VisitConstant(Expression.Constant(Expression.Lambda(memberExpression).Compile().DynamicInvoke()));

            _wherebuilder.Add(col);            
			return base.VisitMemberAccess( memberExpression );
		}

		protected override Expression VisitBinary(BinaryExpression b)
		{
			_wherebuilder.AddBinary( b.NodeType );
			var ret = base.VisitBinary(b);
			_wherebuilder.CompleteBinary();
			return ret;
		}
		
		protected override Expression VisitConstant(ConstantExpression c)
		{
			if ((c.Value is ValueType) || (c.Value is string))
				_wherebuilder.Add( c.Value );

			return base.VisitConstant(c);
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
            return VisitConstant(Expression.Constant(Expression.Lambda(m).Compile().DynamicInvoke()));
		}
	}
}