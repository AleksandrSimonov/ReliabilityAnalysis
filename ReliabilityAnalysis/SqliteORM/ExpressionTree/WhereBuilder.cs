/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SqliteORM.ExpressionTree
{
	public class WhereBuilder
	{
		public class BinaryNode
		{
			public object Left;
			public ExpressionType NodeType;
			public object Right;

			public BinaryNode(ExpressionType nodeType)
			{
				NodeType = nodeType;
			}

			public Where ToWhere()
			{
                if (Left is BinaryNode)
                    Left = ((BinaryNode)Left).ToWhere();

                if (Right is BinaryNode)
                    Right = ((BinaryNode)Right).ToWhere();

				switch (NodeType)
				{
					case ExpressionType.AndAlso:
                        return Where.And( (Where)Left, (Where)Right );
					case ExpressionType.OrElse:
                        return Where.Or( (Where)Left, (Where)Right );
                    case ExpressionType.Divide:
                        return new SqlOperation(string.Format("{0} / {1}",  Left, Right ));
					case ExpressionType.Equal:
						return Where.Equal( (TableColumn) Left, Right);
					case ExpressionType.NotEqual:
                        return Where.NotEqual((TableColumn)Left, Right);
					case ExpressionType.GreaterThan:
                        return Where.GreaterThan((TableColumn)Left, Right);
					case ExpressionType.GreaterThanOrEqual:
                        return Where.GreaterOrEqual((TableColumn)Left, Right);
					case ExpressionType.LessThan:
                        return Where.LessThan((TableColumn)Left, Right);
                    case ExpressionType.LessThanOrEqual:
                        return Where.LessOrEqual( (TableColumn)Left, Right );                    
                    default:
						throw new NotImplementedException("There is no support for the " + NodeType + " expression");
				}
			}
		}

		public BinaryNode Current;

		private readonly Stack<BinaryNode> _stack = new Stack<BinaryNode>();

		public void AddBinary(ExpressionType nodeType)
		{
			if (Current != null)
				_stack.Push( Current );

			Current = new BinaryNode( nodeType );
		}
		
		public void Add(object value)
		{
            if (Current == null)
                throw new Exception( "No Current BinaryNode" );

            if (Current.Left == null)
                Current.Left = value;
            else if (Current.Right == null)
                Current.Right = value;
            else
                throw new Exception("Too many values in expression. Fell over with " + value);
		}

		public void CompleteBinary( )
		{
			if (!_stack.Any())
				return;
            
			BinaryNode node = _stack.Pop();
			if (node.Left == null)
				node.Left = Current;
			else
				node.Right = Current;

			Current = node;
		}
	}
}