
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Ghost.Script
{ 
	internal enum Token
	{
		Keyword = 0,	// 以字母开始的标识符
		Identify, 		// 字母、数字、下划线，不能以数字作为起始字符
		Operator,		// 非字母、数字、下划线的字符

		ConstantBegin_,
		Int8,			// 后缀t
		UInt8,			// 后缀ut
		Int16,			// 后缀s
		UInt16,			// 后缀us
		Int32,			
		UInt32,			// 后缀u
		Int64,			// 后缀l
		UInt64,			// 后缀ul
		Float,			// 后缀f
		Double,
		String,			// "",''
		ConstantEnd_,
	}

	internal enum Keyword
	{
		TypeBegin_ = 1000,
		Int8,			// int8
		Uint8,			// uint8
		Int16,			// int16
		Uint16,			// uint16
		Int32,			// int32
		Uint32,			// uint32
		Int64,			// int64
		Uint64,			// uint64
		Float,			// float
		Double,			// double
		String,			// string
		Bool,			// bool
		TypeEnd_,

		ConstantBegin_,
		Null,			// null
		True,			// true
		False,			// false
		ConstantEnd_,

		If,				// if
		Else,			// else
		Elseif,			// elseif
		Loop,			// loop
		Until,			// until
		End,			// end
		Block,			// block
		Return,			// return
		Class,			// class
		Interface,		// interface
		Static,			// static
		API, 			// API（用于访问外部接口）
		Export 			// export（导出接口给外部访问，只能是全局函数或类静态函数）
	}

	internal enum Operator
	{
		Add = 2000,      // +
		AddAdd,          // ++
		AddAsign,        // +=
		Sub,             // -
		SubSub,          // --
		SubAsign,        // -=
		Mul,             // *
		MulAsign,        // *=
		Div,             // /
		DivAsign,        // /=
		Mod,             // %
		ModAsign,        // %=
		Asign,           // =
		BitAnd,          // &
		BitAndAsign,     // &=
		BitOr,           // |
		BitOrAsign,      // |=
		BitXor,          // ^
		BitXorAsign,     // ^=
		BitRevert,       // ~
		LogicAnd,        // &&
		LogicOr,         // ||
		LogicNot,        // !
		Less,            // <
		LessEqual,       // <=
		Greater,         // >
		GreaterEqual,    // >=
		Equal,           // == 
		NotEqual,        // !=
		Dot,             // .
		Colon,           // :
		Comma,           // ,
		S_Brackets_Beg,  // (
		S_Brackets_End,  // )
		M_Brackets_Beg,  // [
		M_Brackets_End,  // ]
		L_Brackets_Beg,  // {
		L_Brackets_End,  // }
	}

	internal class LexException : Exception
	{
		public Lex.Error errorCode{get;private set;}
		public string content;
		public string line;
		public int row = 0; // start with 1
		public int col = 0; // start with 1
		public char c = '\0';

		public LexException(Lex.Error e)
		{
			errorCode = e;
		}
	}

	internal static class Lex
	{
		[StructLayout(LayoutKind.Explicit, Size = 8)]
		public struct Number
		{
			[FieldOffset(0)]
			public sbyte i_8;
			[FieldOffset(0)]
			public byte ui_8;
			[FieldOffset(0)]
			public short i_16;
			[FieldOffset(0)]
			public ushort ui_16;
			[FieldOffset(0)]
			public int i_32;
			[FieldOffset(0)]
			public uint ui_32;
			[FieldOffset(0)]
			public long i_64;
			[FieldOffset(0)]
			public ulong ui_64;

			[FieldOffset(0)]
			public float f;
			[FieldOffset(0)]
			public double d;
		}
		public class Data
		{
			public Token token;
			public Number number;
			public string str;

			public string line;
			public int row = 0; // start with 1
			public int col = 0; // start with 1

			public Data(){}

			public Data(Token t){token = t;}
			public Data(Token t, string s){token = t;str = s;}
			public Data(Token t, sbyte v){token = t;number.i_8 = v;}
			public Data(Token t, byte v){token = t;number.ui_8 = v;}
			public Data(Token t, short v){token = t;number.i_16 = v;}
			public Data(Token t, ushort v){token = t;number.ui_16 = v;}
			public Data(Token t, int v){token = t;number.i_32 = v;}
			public Data(Token t, uint v){token = t;number.ui_32 = v;}
			public Data(Token t, long v){token = t;number.i_64 = v;}
			public Data(Token t, ulong v){token = t;number.ui_64 = v;}
			public Data(Token t, float v){token = t;number.f = v;}
			public Data(Token t, double v){token = t;number.d = v;}
			public Data(Token t, Keyword v){token = t;number.i_32 = (int)v;}
			public Data(Token t, Operator v){token = t;number.i_32 = (int)v;}
		}

		public enum Error
		{
			None,
			InvalidCharactor,
			StringNoEnd,
			NoteNoEnd,
			InvalidToken,
			ParseNumberFailed,
		}

		public enum ParsePhase{
			New,
			Note,
		}

		private delegate bool NumberParseFunc<T>(string str, out T v);
		private static T DoTryParseNumber<T>(Token token, string str, NumberParseFunc<T> parseFunc)
		{
			T v;
			if (parseFunc(str, out v))
			{
				return v;
			}
			else
			{
				var exception = new LexException(Error.ParseNumberFailed);
				exception.content = str;
				exception.col = (int)token;
				throw exception;
			}
		}

		public static Number TryParseNumber(Token token, string str)
		{
			var number = new Number();
			switch (token)
			{
			case Token.Int8:
				number.i_8 = DoTryParseNumber<sbyte>(token, str, sbyte.TryParse);
				break;
			case Token.UInt8:
				number.ui_8 = DoTryParseNumber<byte>(token, str, byte.TryParse);
				break;
			case Token.Int16:
				number.i_16 = DoTryParseNumber<short>(token, str, short.TryParse);
				break;
			case Token.UInt16:
				number.ui_16 = DoTryParseNumber<ushort>(token, str, ushort.TryParse);
				break;
			case Token.Int32:
				number.i_32 = DoTryParseNumber<int>(token, str, int.TryParse);
				break;
			case Token.UInt32:
				number.ui_32 = DoTryParseNumber<uint>(token, str, uint.TryParse);
				break;
			case Token.Int64:
				number.i_64 = DoTryParseNumber<long>(token, str, long.TryParse);
				break;
			case Token.UInt64:
				number.ui_64 = DoTryParseNumber<ulong>(token, str, ulong.TryParse);
				break;
			case Token.Float:
				number.f = DoTryParseNumber<float>(token, str, float.TryParse);
				break;
			case Token.Double:
				number.d = DoTryParseNumber<double>(token, str, double.TryParse);
				break;
			default:
				{
					var exception = new LexException(Error.InvalidToken);
					exception.content = token.ToString();
					throw exception;
				}
			}
			return number;
		}

		private static int IndexOfStringEnd(string line, int startIndex, char endChar)
		{
			while (startIndex < line.Length)
			{
				var endIndex = line.IndexOf(endChar, startIndex);
				if (0 <= endIndex)
				{
					if ('\\' == line[endIndex-1])
					{
						var slashCount = 1;
						for (int i = endIndex-2; i >= startIndex; --i)
						{
							if ('\\' != line[i])
							{
								break;
							}
							++slashCount;
						}
						if (0 == slashCount%2)
						{
							return endIndex;
						}
						else
						{
							startIndex = endIndex+1;
						}
					}
					else
					{
						return endIndex;
					}
				}
				else
				{
					break;
				}
			}
			return -1;
		}

		private static int IndexOfIdentifyEnd(string line, int startIndex)
		{
			int endIndex = -1;
			for (int i = startIndex; i < line.Length; ++i)
			{
				var c = line[i];
				if ('_' == c || char.IsLetterOrDigit(c))
				{
					endIndex = i;
				}
				else
				{
					break;
				}
			}
			return endIndex;
		}

		private delegate Data ParseFunc(string line, ref int i, ref ParsePhase phase);

		private static string ParseFunc_String(string line, ref int i, ref ParsePhase phase, char separator)
		{
			var begIndex = i+1;
			var endIndex = IndexOfStringEnd(line, begIndex, separator);
			if (i < endIndex)
			{
				var str = line.Substring(begIndex, endIndex-i);
				i = endIndex;
				return str;
			}
			else
			{
				var exception = new LexException(Error.StringNoEnd);
				exception.col = i+1;
				throw exception;
			}
		}

		private static string ParseFunc_Identify(string line, ref int i, ref ParsePhase phase, char c)
		{
			string identify = null;
			var endIndex = IndexOfIdentifyEnd(line, i+1);
			if (i < endIndex)
			{
				identify = line.Substring(i, endIndex-i+1);
				i = endIndex;
			}
			else
			{
				identify = c.ToString();
			}
			return identify;
		}

		private static Data ParseFunc_Number(string line, ref int i, ref ParsePhase phase, char c)
		{
			/*
			 * number phase
			 * 0: scan number
			 * 1: scan float or double
			 * 2: scan end char
			 * 3: check next char is not(letter,digit,_)
			 */ 
			Token token = Token.Int32;
			int numberPhase = 0;
			int j = i+1;
			for (; j < line.Length; ++j)
			{
				var d = line[j];
				switch (numberPhase)
				{
				case 0:
					switch (d)
					{
					case 't':
						numberPhase = 3;
						token = Token.Int8;
						continue;
					case 's':
						numberPhase = 3;
						token = Token.Int16;
						continue;
					case 'l':
						numberPhase = 3;
						token = Token.Int64;
						continue;
					case 'f':
						numberPhase = 3;
						token = Token.Float;
						continue;
					case 'u':
						numberPhase = 2;
						token = Token.UInt32;
						continue;
					case '.':
						numberPhase = 1;
						token = Token.Double;
						continue;
					}
					break;
				case 1:
					if ('f' == d)
					{
						numberPhase = 3;
						token = Token.Float;
						continue;
					}
					break;
				case 2:
					switch (d)
					{
					case 't':
						numberPhase = 3;
						token = Token.UInt8;
						continue;
					case 's':
						numberPhase = 3;
						token = Token.UInt16;
						continue;
					case 'l':
						numberPhase = 3;
						token = Token.UInt64;
						continue;
					}
					break;
				case 3:
					if ('_' == d || '.' == d || char.IsLetterOrDigit(d))
					{
						var exception = new LexException(Error.InvalidCharactor);
						exception.col = j+1;
						throw exception;
					}
					else
					{
						// end
						var data = new Data(token);
						data.number = TryParseNumber(token, line.Substring(i, j-i));
						i = j-1;
						return data;
					}
				}
				if (!char.IsDigit(d))
				{
					if ('_' == d || '.' == d || char.IsLetter(d))
					{
						var exception = new LexException(Error.InvalidCharactor);
						exception.col = j+1;
						throw exception;
					}
					else
					{
						// end
						var data = new Data(token);
						data.number = TryParseNumber(token, line.Substring(i, j-i));
						i = j-1;
						return data;
					}
				}
			}
			return null;
		}

		private static Dictionary<char,ParseFunc> StartCharMap = new Dictionary<char,ParseFunc>{
			{'/',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('/' == line[i+1])
						{
							#if LOG_NOTE
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.WriteLine(line.Substring(i));
							#endif
							i = line.Length;
							return null;
						}
						else if ('*' == line[i+1])
						{
							var endIndex = line.IndexOf("*/");
							if (i < endIndex)
							{
								#if LOG_NOTE
								Console.ForegroundColor = ConsoleColor.Gray;
								Console.WriteLine(line.Substring(i, endIndex-i+2));
								#endif
								i = endIndex+1;
							}
							else
							{
								#if LOG_NOTE
								Console.ForegroundColor = ConsoleColor.Gray;
								Console.WriteLine(line.Substring(i));
								#endif
								i = line.Length;
								phase = ParsePhase.Note;
							}
							return null;
						}
						else if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.DivAsign);
						}
					}
					return new Data(Token.Operator, Operator.Div);
				}
			},  // /,//,/*,/=
			{'\"',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.String, ParseFunc_String(line, ref i, ref phase, '\"'));}}, // ""
			{'\'',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.String, ParseFunc_String(line, ref i, ref phase, '\''));}}, // ''
			{'+',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('+' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.AddAdd);
						}
						else if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.AddAsign);
						}
					}
					return new Data(Token.Operator, Operator.Add);
				}
			},  // +,++,+=
			{'-',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('-' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.SubSub);
						}
						else if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.SubAsign);
						}
					}
					return new Data(Token.Operator, Operator.Sub);
				}
			},  // -,--,-=
			{'*',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.MulAsign);
						}
					}
					return new Data(Token.Operator, Operator.Mul);
				}
			},  // *,*=
			{'%',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.ModAsign);
						}
					}
					return new Data(Token.Operator, Operator.Mod);
				}
			},  // %,%=
			{'=',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.Equal);
						}
					}
					return new Data(Token.Operator, Operator.Asign);
				}
			},  // =,==
			{'&',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('&' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.LogicAnd);
						}
						else if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.BitAndAsign);
						}
					}
					return new Data(Token.Operator, Operator.BitAnd);
				}
			},  // &,&=,&&
			{'|',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('|' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.LogicOr);
						}
						else if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.BitOrAsign);
						}
					}
					return new Data(Token.Operator, Operator.BitOr);
				}
			},  // |,|=,||
			{'^',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.BitXorAsign);
						}
					}
					return new Data(Token.Operator, Operator.BitXor);
				}
			},  // ^,^=
			{'<',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.LessEqual);
						}
					}
					return new Data(Token.Operator, Operator.Less);
				}
			},  // <,<=
			{'>',delegate(string line, ref int i, ref ParsePhase phase){
					if (i < line.Length-1)
					{
						if ('=' == line[i+1])
						{
							++i;
							return new Data(Token.Operator, Operator.GreaterEqual);
						}
					}
					return new Data(Token.Operator, Operator.Greater);
				}
			},  // >,>=
			{'.',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.Dot);}},
			{':',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.Colon);}},
			{',',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.Comma);}},
			{'~',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.BitRevert);}},
			{'!',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.LogicNot);}},
			{'(',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.S_Brackets_Beg);}},
			{')',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.S_Brackets_End);}},
			{'[',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.M_Brackets_Beg);}},
			{']',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.M_Brackets_End);}},
			{'{',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.L_Brackets_Beg);}},
			{'}',delegate(string line, ref int i, ref ParsePhase phase){return new Data(Token.Operator, Operator.L_Brackets_End);}},
		};

		static Lex ()
		{
		}

		public static ParsePhase ParseLine<T>(string line, int lineNumber, ParsePhase phase, System.Action<Data, T> dataReceiver, T custom)
		{
			if (string.IsNullOrEmpty(line))
			{
				return phase;
			}
			int i = 0;
			if (ParsePhase.Note == phase)
			{
				var endIndex = line.IndexOf("*/");
				if (0 > endIndex)
				{
					#if LOG_NOTE
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.WriteLine(line);
					#endif
					return phase;
				}
				#if LOG_NOTE
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine(line.Substring(0, endIndex+2));
				#endif
				phase = ParsePhase.New;
				i = endIndex+2;
			}
			for (; i < line.Length; ++i)
			{
				var startI = i;
				Data data = null;
				var c = line[i];
				ParseFunc func;
				if (StartCharMap.TryGetValue(c, out func))
				{
					if (null != func)
					{
						data = func(line, ref i, ref phase);
					}
				}
				else if ('_' == c)
				{
					// identify
					var identify = ParseFunc_Identify(line, ref i, ref phase, c);
					data = new Data(Token.Identify, identify);
				}
				else if (char.IsLetter(c))
				{
					var identify = ParseFunc_Identify(line, ref i, ref phase, c);
					Keyword k;
					if (Enum.TryParse(identify, true, out k)
						&& ("API" == identify || identify.ToLower() == identify))
					{
						// keyword
						data = new Data(Token.Keyword, k);
					}
					else
					{
						// identify
						data = new Data(Token.Identify, identify);
					}
				}
				else if (char.IsDigit(c))
				{
					// constant
					data = ParseFunc_Number(line, ref i, ref phase, c);
				}
				else if (!char.IsWhiteSpace(c))
				{
					var exception = new LexException(Error.InvalidCharactor);
					exception.col = i+1;
					throw exception;
				}
				if (null != data)
				{
					data.line = line;
					data.row = lineNumber;
					data.col = startI+1;
					dataReceiver(data, custom);
				}
			}
			return phase;
		}

		public static void ParseStream<T>(StreamReader reader, System.Action<Data, T> dataReceiver, T custom)
		{
			var phase = ParsePhase.New;
			int lineNumber = 1;
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				#if LOG_LINE
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(String.Format("TryParseLine[{0}]\n{1}", lineNumber, line));
				#endif 
				try
				{
					phase = ParseLine(line, lineNumber, phase, dataReceiver, custom);
				}
				catch (LexException e)
				{
					e.line = line;
					e.row = lineNumber;
					throw e;
				}
				++lineNumber;
			}
			if (ParsePhase.Note == phase)
			{
				var exception = new LexException(Error.NoteNoEnd);
				exception.content = "No \"*/\"";
				throw exception;
			}
		}
	}
}

