/*
 *  Sqlite ORM - PUBLIC DOMAIN LICENSE
 *  Copyright (C)  2010-2012. Ian Quigley
 *  
 *  This source code is provided 'As is'. You bear the risk of using it.
 */

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace SqliteORM
{
	public class SchemaParse : DbConnection
	{
		public static TableMeta ToMeta(string tableName)
		{
            // Parse a string like;
            // CREATE TABLE SimpleTable ([Id] long not null,[Test] nvarchar(300) ,[When] date , PRIMARY KEY ([Id] ))
			
            TableMeta meta = new TableMeta {ParameterizedTableName = tableName};

			string schema = GetDbSchemaFor( tableName );
			if (string.IsNullOrEmpty( schema ))
				return null;

			StringParse parse = new StringParse( schema );
			if (!parse.Read("CREATE TABLE " + tableName + " ("))
				return null;
			
			while (!parse.Peek(")") && !parse.IsEmpty)
			{
                parse.SkipWhitespace();


				if (parse.Peek("[") || parse.Contains(' '))
				{
                    string fieldName;
                    if (parse.Peek("["))
                    {
                        fieldName = parse.ReadBetween('[', ']');
                    }
                    else
                    {
                        if (parse.Peek("PRIMARY KEY"))
                            break;

                        fieldName = parse.ReadTo(' ');                        
                    }

                    parse.SkipWhitespace();

					TableColumn tc = new TableColumn
					                 	{
					                 		Name = fieldName,
					                 		Get = (t, inst) => ((AnonmousTable) inst)[ t.RawName ],
					                 		Set = (t, inst, val) => ((AnonmousTable) inst)[ t.RawName ] = val
					                 	};

					parse.While( KeywordTriggerDefinitions, tc );

                    meta.Columns.Add(tc);
				}

                if (parse.Peek(","))
                {
                    parse.Read(",");
                    continue;
                }

                if (parse.Peek(")"))
                    break;

                if (parse.Peek("PRIMARY KEY"))
                    break;
             
  				throw new NotImplementedException("Incomplete parsing of " + tableName);
			}

			return meta;
		}

        private static readonly List<TriggerAction> KeywordTriggerDefinitions = new List<TriggerAction>()
     	{
			new TriggerAction( "not null", (sp,tc)  => tc.IsNullable = false),
            new TriggerAction( "autoincrement", (sp,tc) => tc.AutoIncrement = true),
			new TriggerAction( "primary key", (sp,tc) => tc.PrimaryKey = true),
			new TriggerAction( "default", (sp,tc) => sp.SkipPast( ',' )),

            new TriggerAction("bit", (sp, tc) => tc.Type = typeof(Boolean)),
            new TriggerAction("char", (sp, tc) => tc.Type = typeof(Char)),
            new TriggerAction("byte", (sp, tc) => tc.Type = typeof(Byte)),
            new TriggerAction("smallint", (sp, tc) => tc.Type = typeof(Int16)),
            new TriggerAction("int", (sp, tc) => tc.Type = typeof(Int32)),
			new TriggerAction("integer", (sp, tc) => tc.Type = typeof(Int32)),
            new TriggerAction("long", (sp, tc) => tc.Type = typeof(Int64)),
            new TriggerAction("real", (sp, tc) => tc.Type = typeof(Single)),
            new TriggerAction("double", (sp, tc) => tc.Type = typeof(Double)),
            new TriggerAction("double", (sp, tc) => tc.Type = typeof(Decimal)),
            new TriggerAction("date", (sp, tc) => tc.Type = typeof(DateTime)),
            new TriggerAction("object", (sp, tc) => tc.Type = typeof(TimeSpan)),
            new TriggerAction("guid", (sp, tc) => tc.Type = typeof(Guid)),
            new TriggerAction("ntext", (sp, tc) => tc.Type = typeof(String)),
            new TriggerAction("text", (sp, tc) => tc.Type = typeof(String)),
            new TriggerAction("nvarchar", (sp, tc) => {tc.Type = typeof (string); sp.SkipPast(')'); } ),
            new TriggerAction("varchar", (sp, tc) => {tc.Type = typeof (string); sp.SkipPast(')'); } )
        };
        

		private class TriggerAction
		{
			public string Trigger;
            public Action<StringParse, TableColumn> Action;

            public TriggerAction(string trigger, Action<StringParse, TableColumn> action)
			{
				Trigger = trigger;
				Action = action;
			}
		}

		private static string GetDbSchemaFor(string tableName)
		{
			using (SQLiteCommand command = new SQLiteCommand("SELECT sql FROM sqlite_master WHERE type='table' and name = @p1;", (new SchemaParse()).Connection))
			{
				command.Parameters.AddWithValue( "@p1", tableName );
				return command.ExecuteScalar() as string;
			}
		}

		private class StringParse
		{
			private readonly string _text;
			private int _index;

			internal StringParse(string text)
			{
				_text = text;
				_index = 0;
			}

			public override string ToString()
			{
				if (IsEmpty)
					return "**empty**";
				
				int x = (_index < 10) ? 10 : _index;
				return _text.Substring( _index - x, x ) + ">" + _text.Substring( _index, 40 );
			}
			internal bool Read(string expect)
			{
				bool b = Peek( expect );
				_index += expect.Length;
				return b;
			}

			internal bool IsEmpty { get { return _index >= _text.Length; } }

			internal bool Peek(string peek)
			{
				if (peek.Length > _text.Length - _index)
					return false;

				return (_text.Substring( _index, peek.Length ) == peek);
			}

			internal string ReadTo(char c)
			{
				int _start = _index;
				while (_index < _text.Length && _text[_index] != c) _index++;
				return _text.Substring( _start, _index - _start );
			}

            internal void SkipPast(char p)
            {
                while (_index < _text.Length && _text[_index++] != p) { };
            }

			internal void While(IEnumerable<TriggerAction> stringActions, TableColumn tc)
			{
                int shortest = stringActions.Min(t => t.Trigger.Length);
                if (_index + shortest > _text.Length)
                    return;

                while (_index + shortest < _text.Length)
                {
                    string peek = _text.Substring(_index, shortest);
					
					var shortlist = stringActions.Where( t => t.Trigger.StartsWith( peek ) );

                    while (shortlist.Count() > 1 && _index + shortest < _text.Length)
                    {
                        shortest++;
                        peek = _text.Substring(_index, shortest);


						shortlist = stringActions.Where( t => t.Trigger.StartsWith(peek));
						if (shortlist.Count() == 1)
							break;

						if (shortlist.Count() == 0)	
						{
							shortlist = stringActions.Where( t => t.Trigger == _text.Substring( _index, shortest -1 ) );
							if (shortlist.Count() == 1)
								break;
						}
                    }

                    if (shortlist.Count() != 1)
                        return;

                    var match = shortlist.First();
                    if (!Peek(match.Trigger))
                        return;

                    _index += match.Trigger.Length;

                    // do the action associated with the string
                    match.Action(this, tc);

                    SkipWhitespace();
                }
			}

            internal void SkipWhitespace()
            {
                while (_index < _text.Length && Char.IsWhiteSpace(_text[_index])) _index++;
            }

			internal string ReadBetween(char start, char end)
			{
				if (!Read(start.ToString()))
					return null;

				string val = ReadTo( end );
				Read( end.ToString() );

				return val;
			}

            internal bool Contains(char c)
            {
                return _text.IndexOf(c, _index) != -1;
            }
        }
	}
}
