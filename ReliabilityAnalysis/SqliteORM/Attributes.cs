/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;

namespace SqliteORM
{
	public class TableAttribute : Attribute
	{
		public string Name { get; set; }

		public TableAttribute()
		{ }

		public TableAttribute(string name)
		{
			Name = name;
		}
	}

	public class FieldAttribute : Attribute
	{
		public bool AutoIncrement { get; set; }
		public bool PrimaryKey { get; set; }
		public bool IsNullable { get; set; }
		public string Name { get; set; }
		public bool IsCaseInsensitive { get; set; }

        public FieldAttribute()
            : this(false, true, null)
        { }

        public FieldAttribute(string name)
            : this(false, true, name)
        { }
                
		public FieldAttribute( bool primaryKey, bool isNullable, string name, bool autoIncrement = false, bool isCaseInsensitive = false )
		{
			Name = name;
			PrimaryKey = primaryKey;
			IsNullable = isNullable;			
			AutoIncrement = autoIncrement;
            IsCaseInsensitive = isCaseInsensitive;
		}
	}

    public class ForeignKeyAttribute : FieldAttribute
    {
        public Type ForeignTableType;

        public ForeignKeyAttribute(Type table)
        {
            ForeignTableType = table;
        }
    }

	public class PrimaryKeyAttribute : FieldAttribute
	{
		public PrimaryKeyAttribute( )
			: base( true, false, null)
		{ }

		public PrimaryKeyAttribute( bool autoIncrement)
			: base( true, false, null, autoIncrement )
		{ }

		public PrimaryKeyAttribute( string name)
            : base(true, false, name)
        { }
	}
}