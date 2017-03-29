using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ghost.Script
{
	internal class SyntaxException : Exception
	{
		public Syntax.Error errorCode{get;private set;}
		public string content;
		public string line;
		public int row = 0;
		public int col = 0;

		public SyntaxException(Syntax.Error e)
		{
			errorCode = e;
		}
	}

	internal static class Syntax
	{
		public enum Error
		{
			None,
			NotComplete,
			InvalidToken,
			InvalidKeyword,
			InvalidOperator,
			InvalidFunction,
		}

		public class Context
		{
			public Syntax_Node node;

			public Context(Syntax_Node root)
			{
				node = root;
			}
		}

		public static List<Token> constantTokenList{get;internal set;}
		public static List<Keyword> typeKeywordList{get;internal set;}
		public static List<Keyword> constantKeywordList{get;internal set;}

		static Syntax()
		{
			constantTokenList = new List<Token>();
			for (int i = (int)Token.ConstantBegin_+1; i < (int)Token.ConstantEnd_; ++i)
			{
				constantTokenList.Add((Token)i);
			}

			typeKeywordList = new List<Keyword>();
			for (int i = (int)Keyword.TypeBegin_+1; i < (int)Keyword.TypeEnd_; ++i)
			{
				typeKeywordList.Add((Keyword)i);
			}

			constantKeywordList = new List<Keyword>();
			for (int i = (int)Keyword.ConstantBegin_+1; i < (int)Keyword.ConstantEnd_; ++i)
			{
				constantKeywordList.Add((Keyword)i);
			}
		}

		public static void Log_LexData(Lex.Data data)
		{
			switch (data.token)
			{
			case Token.Keyword:
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine((Keyword)data.number.i_32);
				break;
			case Token.Identify:
				Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine(data.str);
				break;
			case Token.Operator:
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine((Operator)data.number.i_32);
				break;
			case Token.Int8:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.i_8);
				break;
			case Token.UInt8:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.ui_8);
				break;
			case Token.Int16:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.i_16);
				break;
			case Token.UInt16:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.ui_16);
				break;
			case Token.Int32:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.i_32);
				break;
			case Token.UInt32:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.ui_32);
				break;
			case Token.Int64:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.i_64);
				break;
			case Token.UInt64:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.ui_64);
				break;
			case Token.Float:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.f);
				break;
			case Token.Double:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine(data.number.d);
				break;
			case Token.String:
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(data.str);
				break;
			}
		}
			
		internal static void LexDataReceiver(Lex.Data data, Context context)
		{
			#if LOG_LEX
			Log_LexData(data);
			#endif
			try
			{
				context.node = context.node.ParseLex(data);
			}
			catch (SyntaxException e)
			{
				e.line = data.line;
				e.row = data.row;
				e.col = data.col;
				throw e;
			}
		}

		public static void ParseStream(StreamReader reader, string path = null)
		{
			var root = Syntax_Node.Create<Syntax_File>();
			root.path = path;
			var context = new Context(root);
			Lex.ParseStream(reader, LexDataReceiver, context);
			if (!(context.node is Syntax_File))
			{
				var exception = new SyntaxException(Error.NotComplete);
				exception.content = context.node.ToString();
				throw exception;
			}
		}
	}
}