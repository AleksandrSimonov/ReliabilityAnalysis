/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqliteORM.Dialect
{
	public class Actions
	{
		internal static string CreateTable(TableMeta meta, string tableName)
		{
			if ( (meta.Columns.Count( col => col.PrimaryKey && col.AutoIncrement ) == 1) ||
                 (meta.Columns.Count( col => col.PrimaryKey) == 0))
				return String.Format("CREATE TABLE if not exists {0} ({1}); ", tableName,
				                      BuildColumnList( meta.Columns, (sb, col) => sb.AppendFormat( "{0} {1}", col.Name, col.SqlType ) ) );


			return String.Format("CREATE TABLE if not exists {0} ({1}, PRIMARY KEY ({2})); ", tableName,
			                     BuildColumnList(meta.Columns, (sb, col) => sb.AppendFormat("{0} {1}", col.Name, col.SqlType)),
			                     BuildColumnList(meta.Columns.Where(col => col.PrimaryKey) , (sb, col) => sb.AppendFormat("{0} ", col.Name)));
		}


		internal static string UpdateSql(TableMeta meta, string tableName)
		{
			return string.Format("INSERT or REPLACE INTO {0} ({1}) VALUES ({2}); ", tableName,
			                     BuildColumnList(meta.Columns, (sb, col) => sb.AppendFormat("{0}", col.Name)),
			                     BuildColumnList(meta.Columns, (sb, col) => sb.AppendFormat("{0}", col.ParamName)));                
		}

		internal static string SelectRowSql(TableMeta meta, string tableName) 
		{
			return string.Format("SELECT {0} FROM {1} WHERE {2} ",
			                     BuildColumnList(meta.Columns, (sb, col) => sb.AppendFormat("{0}", col.Name)),
								 tableName,
			                     BuildColumnList(meta.Columns.Where(col => col.PrimaryKey), (sb, col) => sb.AppendFormat("{0} = {1}", col.Name, col.ParamName), " AND "));
		}

		internal static string SelectAllSql(TableMeta meta, string tableName) 
		{
			return string.Format("SELECT {0} FROM {1}",
			                     BuildColumnList(meta.Columns, (sb, col) => sb.AppendFormat("{0}", col.Name)),
								 tableName);
		}
                
		internal static string DeleteRowSql(TableMeta meta, string tableName)
		{
			return string.Format("DELETE FROM {0} WHERE {1}", tableName,
			                     BuildColumnList(meta.Columns.Where(col => col.PrimaryKey), (sb, col) => sb.AppendFormat("{0} = {1}", col.Name, col.ParamName), " AND "));
		}

		internal static string DeleteSql(TableMeta meta, string tableName) 
		{
			return string.Format("DELETE FROM {0}", tableName);
		}

		private static string BuildColumnList(IEnumerable<TableColumn> columns, Action<StringBuilder, TableColumn> builder)
		{
			return BuildColumnList(columns, builder, ",");
		}

		private static string BuildColumnList(IEnumerable<TableColumn> columns, Action<StringBuilder, TableColumn> builder, string join)
		{
			if (!columns.Any())
				return string.Empty;

			StringBuilder list = new StringBuilder();
			foreach (TableColumn column in columns)
			{
				builder(list, column);
				list.Append(join);
			}

			list.Remove(list.Length - join.Length, join.Length);
			return list.ToString();
		}

        internal static string ColumnsWithAlias(TableMeta meta, string alias)
        {
            return BuildColumnList(meta.Columns, (sb, col) => sb.AppendFormat("{0}.{1}", alias, col.Name));
        }

        internal static string SelectAllJoinSql(List<TableMeta> list, string[] tableAlias)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            
            for (int i = 0; i < list.Count(); i++)
                sql.Append(BuildColumnList(list[i].Columns, (sb, col) => sb.AppendFormat("{0}.{1}", tableAlias[i], col.Name)) + ", ");

            sql.Remove(sql.Length - 2, 2);
            
            sql.AppendFormat(" FROM {0} as {1} ", list[0].ParameterizedTableName, tableAlias[0]);
            for (int i = 1; i < list.Count(); i++)
                sql.AppendFormat("JOIN {0} as {1} on {2} ",
                    list[i].ParameterizedTableName, tableAlias[i],
                    JoinOn(list[i-1], tableAlias[i-1], list[i], tableAlias[i]));

            return sql.ToString();
        }

        private static string JoinOn(TableMeta metaA, string aliasA, TableMeta metaB, string aliasB)
        {
            var join = FindJoin(metaA, metaB);
            if (join != null)
                return string.Format("{0}.{1} == {2}.{3}", aliasA, join.Name, aliasB, metaB.Columns.First(c => c.PrimaryKey).Name);
           
            join = FindJoin(metaB, metaA);
            if (join != null)
                return string.Format("{0}.{1} == {2}.{3}", aliasB, join.Name, aliasA, metaA.Columns.First(c => c.PrimaryKey).Name);

            join = (from colA in metaA.Columns
                    join colB in metaB.Columns on colA.ParentTableType equals colB.ParentTableType
                    select colA).FirstOrDefault();

            if (join != null)
                return string.Format("{0}.{1} == {2}.{3}", aliasA, join.Name, aliasB, metaA.Columns.First(c => c.ParentTableType == join.ParentTableType ).Name);

            throw new ArgumentException( "There is no ForeignKey relationship between entities " + metaA.TableType.Name + " and " + metaB.TableType.Name );
        }


        /// <summary>
        /// Find ForeignKey in Meta-B which relates to Meta-A
        /// </summary>
        /// <param name="metaA"></param>
        /// <param name="metaB"></param>
        /// <returns></returns>
        private static TableColumn FindJoin(TableMeta metaA, TableMeta metaB)
        {
            return metaA.Columns.FirstOrDefault(t => t.IsForeignKey && t.ParentTableType == metaB.TableType);
        }
    }
}