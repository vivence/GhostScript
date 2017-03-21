using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
		}

		public enum ParsePhase{
			None,
		}

		public class Context
		{
			public ParsePhase phase = ParsePhase.None;
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
			#if DEBUG
			Log_LexData(data);
			#endif

			switch (context.phase)
			{
			case ParsePhase.None:
				break;
			}
		}

		public static void ParseStream(StreamReader reader)
		{
			var context = new Context();
			Lex.ParseStream(reader, LexDataReceiver, context);
		}
	}
}

//GlobalFunction()
//a = 1 // local variable
//b = 2l
//c = a+b
//XXClass.abc = "abc"+"def"+1+1.1 // class static variable
//obj.d = true // class instance variable
//a = Func(a,b,c)
//a,b = Func_(obj.d, XXClass.abc)
//if ()
//elseif ()
//	else
//		end
//		loop()
//		end
//		loop
//		until()
//		end
//
//		LocalFunction_()
//		block
//
//		end
//		return 1,2
//end
//
//class id:BaseClass(interface)
//int32 privateVariable_
//int64 publicVariableB = 0l
//
//id() // 构造，系统行为，不可访问
//end
//~id() // 析构，系统行为，不可访问
//end
//
//static
//bool staticPrivateVariable_
//
//id() // 静态构造，系统行为，不可访问
//end
//
//StaticPublicFunction()
//end
//end
//end
//
//interface
//end

