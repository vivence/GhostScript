using System.Text;
using System;

namespace Ghost.Script
{
	internal class Syntax_File : Syntax_Node
	{
		// TODO 将使用到的外部引用存储为字典，后期建立引用分配的时候进行分配赋值。
		public string path{get;set;}

		#region override
		internal override void DoToString(StringBuilder sb)
		{
			sb.AppendFormat(".{0}(File)", path);
		}

		internal override void DoConstruct(Lex.Data data)
		{
			
		}

		internal override void DoDeconstruct()
		{
			
		}

		internal override Syntax_Node ParseLex(Lex.Data data)
		{
			switch (data.token)
			{
			case Token.Keyword:
				{
					var keyword = (Keyword)data.number.i_32;
					switch (keyword)
					{
					case Keyword.Interface:
						return AddChild(Syntax_Node.Create<Syntax_Interface>(data, this));
					case Keyword.Class:
						return AddChild(Syntax_Node.Create<Syntax_Class>(data, this));
					default:
						{
							var exception = new SyntaxException(Syntax.Error.InvalidKeyword);
							exception.content = keyword.ToString();
							throw exception;
						}
					}
				}
			case Token.Identify:
				return AddChild(Syntax_Node.Create<Syntax_Function>(data, this));
			default:
				{
					var exception = new SyntaxException(Syntax.Error.InvalidToken);
					exception.content = data.token.ToString();
					throw exception;
				}
			}
		}
		#endregion override
	}
}

