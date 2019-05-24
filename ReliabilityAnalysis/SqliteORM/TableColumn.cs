/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Data;
using System.Reflection;
using SqliteORM.Dialect;

namespace SqliteORM
{
    public class PropertyFieldInfo
    {
        private PropertyInfo _pi;
        private FieldInfo _fi;

        public PropertyFieldInfo(PropertyInfo pi)
        {
            _pi = pi;
        }

        public PropertyFieldInfo(FieldInfo fi)
        {
            _fi = fi;
        }

        internal object GetValue(object inst)
        {
            return _pi != null
                ? _pi.GetValue(inst, null)
                : _fi.GetValue(inst);
        }

        internal void SetValue(object inst, object value)
        {
            if (_pi != null)
                _pi.SetValue(inst, value, null);
            else
                _fi.SetValue(inst, value);
        }

        internal FieldAttribute FieldAttribute()
        {
            var columnDefinitions = _pi != null
                ? _pi.GetCustomAttributes(typeof(FieldAttribute), true)
                : _fi.GetCustomAttributes(typeof(FieldAttribute), true);
            
            if (columnDefinitions.Length != 1)
                return null;

            return columnDefinitions[0] as FieldAttribute;
        }

        public Type Type
        {
            get
            {
                return _pi != null ? _pi.PropertyType : _fi.FieldType;
            }
        }

        public string Name
        {
            get
            {
                return _pi != null ? _pi.Name : _fi.Name;
            }
        }
    }

	public class TableColumn
	{
        internal readonly PropertyFieldInfo _member;

        public string Name { get { return Column.SafeFieldName(RawName); } set { RawName = value; } }
        public string SqlType { get { return Column.CreateFieldDefinition( Type, PrimaryKey, IsNullable, AutoIncrement, IsCaseInsensitive ); } }
        public DbType DbType { get { return Column.ToDbType(Type); } }
        public string ParamName { get { return Column.ParamName(RawName); } }
		public bool PrimaryKey;
		public bool AutoIncrement;

		public string FieldName;
		public bool IsNullable = true;
        public bool IsForeignKey = false;
        public bool IsCaseInsensitive = false;
        public bool IsFloatingPoint { get { return DbType == System.Data.DbType.Decimal || DbType == System.Data.DbType.Double || DbType == System.Data.DbType.Single; } }
        public Type ParentTableType { get; private set; }

        public string RawName;

		public Type Type;

        public Func<TableColumn, object, object> Get;
        public Action<TableColumn, object, object> Set;
                
		public TableColumn()
		{}
			
		public TableColumn(FieldAttribute definition, PropertyFieldInfo member)
		{
            Name = definition.Name;
			IsNullable = definition.IsNullable;
            
			PrimaryKey = definition.PrimaryKey;
			AutoIncrement = definition.AutoIncrement;
			Type = member.Type;
			_member = member;

            if (definition is ForeignKeyAttribute)
            {
                ParentTableType = ((ForeignKeyAttribute)definition).ForeignTableType;
                IsForeignKey = true;
            }

			FieldName = member.Name;

            Get = (tc, inst) => _member.GetValue(inst);
            Set = (tc, inst, val) => _member.SetValue(inst, val );

            if (Type.IsEnum)
                Get = ( tc, inst ) => (int)_member.GetValue( inst );

            if (Type == typeof(decimal))
                Set = (tc, inst, val) => _member.SetValue(inst, Convert.ToDecimal(val) );
		}

        public TableColumn(TableColumn tc)
        {
        
            _member = tc._member;
		    PrimaryKey = tc.PrimaryKey;
            AutoIncrement = tc.AutoIncrement;

		    FieldName = tc.FieldName;
		    IsNullable = tc.IsNullable;
            IsForeignKey = tc.IsForeignKey;
            ParentTableType  = tc.ParentTableType;

            RawName = tc.RawName;
		    
		    Type = tc.Type;

            Get = tc.Get;
            Set = tc.Set;
        }


		public object GetValue(object inst)
		{
            return Get(this, inst);
		}

		public void SetValue(object inst, object val)
		{
            Set(this, inst, val);
		}

		public bool IsDefaultValue(object instance)
		{
			return (Type.IsValueType && Activator.CreateInstance( Type ).Equals( GetValue( instance ) )) || (instance == null);
		}
	}
}