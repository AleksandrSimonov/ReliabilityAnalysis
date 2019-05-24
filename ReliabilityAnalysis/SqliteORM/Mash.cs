/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace SqliteORM
{
    public interface IMash
    {
        string SelectAllSql();
    }

    public class Mash<T, R> : TableMeta, IMash
    {
		public T Table1 { get; set; }
        public R Table2 { get; set; }

        public Mash()
        {
            Table1 = Activator.CreateInstance<T>();
            TableMeta t = Get(typeof(T));
            Columns.AddRange(
                from tc in t.Columns
                select new TableColumn(tc)
                    {
                        Set = (tx, inst, val) => tc.Set(tx, ((Mash<T, R>)inst).Table1, val)
                    });
               
            Table2 = Activator.CreateInstance<R>();
            TableMeta r = Get(typeof(R));
            Columns.AddRange(
                from tc in r.Columns
                select new TableColumn(tc)
                {
                    Set = (tx, inst, val) => tc.Set(tx, ((Mash<T, R>)inst).Table2, val)
                });
		}

        public string SelectAllSql()
        {
            TableMeta t = TableMeta.Get(typeof(T));
            TableMeta r = TableMeta.Get(typeof(R));

            return Dialect.Actions.SelectAllJoinSql(new List<TableMeta>() { t, r }, new string[] { "t1", "t2" });
        }      
	}

	
    public class Mash<T, R, S> : TableMeta, IMash
    {
        public T Table1 { get; set; }
        public R Table2 { get; set; }
        public S Table3 { get; set; }

        public Mash()
        {
            Table1 = Activator.CreateInstance<T>();
            TableMeta t = Get(typeof(T));
            Columns.AddRange(
                from tc in t.Columns
                select new TableColumn(tc)
                    {
                        Set = (tx, inst,val) => tc.Set(tx, ((Mash<T,R,S>)inst).Table1, val)
                    });
               
            Table2 = Activator.CreateInstance<R>();
            TableMeta r = Get(typeof(R));
            Columns.AddRange(
                from tc in r.Columns
                select new TableColumn(tc)
                {
                    Set = (tx, inst, val) => tc.Set(tx, ((Mash<T, R, S>)inst).Table2, val)
                });

            Table3 = Activator.CreateInstance<S>();
            TableMeta s = Get(typeof(S));
            Columns.AddRange(
                from tc in s.Columns
                select new TableColumn(tc)
                {
                    Set = (tx, inst, val) => tc.Set(tx, ((Mash<T, R, S>)inst).Table3, val)
                });
        }

        public string SelectAllSql()
        {
            TableMeta t = TableMeta.Get(typeof(T));
            TableMeta r = TableMeta.Get(typeof(R));
            TableMeta s = TableMeta.Get(typeof(S));

            return Dialect.Actions.SelectAllJoinSql(new List<TableMeta>() { t, r, s }, new string[] { "t1", "t2", "t3" });
        }      
    }

}
