using System.Text;
using System.Collections.Generic;

namespace Ghost.Script
{
	internal class Syntax_Function : Syntax_CodeBlock
	{
		public enum Phase
		{
			ParamsBegin,
			ParamType,
			ParamName,
			ParamSeparator,
			Code
		}

		public string name{get;internal set;}
		public List<KeyValuePair<Lex.Data, Lex.Data>> paramList{get;internal set;}

		public Phase phase{get;internal set;}

		internal Lex.Data paramType_;

		#region override
		internal override void DoToString(StringBuilder sb)
		{
			sb.AppendFormat(".{0}(Function)", name);
		}

		internal override void DoConstruct(Lex.Data data)
		{
			base.DoConstruct(data);
			name = data.str;
			phase = Phase.ParamsBegin;
			paramType_ = null;
		}

		internal override void DoDeconstruct()
		{
			base.DoDeconstruct();
		}

		internal override Syntax_Node ParseLex(Lex.Data data)
		{
			switch (phase)
			{
			case Phase.ParamsBegin:
				if (Token.Operator == data.token 
					&& Operator.S_Brackets_Beg == (Operator)data.number.i_32)
				{
					phase = Phase.ParamType;
					return this;
				}
				else
				{
					var exception = new SyntaxException(Syntax.Error.InvalidFunction);
					exception.content = "Need\"(\"";
					throw exception;
				}
			case Phase.ParamType:
				switch (data.token)
				{
				case Token.Operator:
					if (Operator.S_Brackets_End == (Operator)data.number.i_32)
					{
						phase = Phase.Code;
						return this;
					}
					else
					{
						var exception = new SyntaxException(Syntax.Error.InvalidOperator);
						exception.content = ((Operator)data.number.i_32).ToString();
						throw exception;
					}
				case Token.Keyword:
					if (Syntax.typeKeywordList.Contains((Keyword)data.number.i_32))
					{
						paramType_ = data;
						phase = Phase.ParamName;
						return this;
					}
					else
					{
						var exception = new SyntaxException(Syntax.Error.InvalidKeyword);
						exception.content = ((Keyword)data.number.i_32).ToString();
						throw exception;
					}
				case Token.Identify:
					paramType_ = data;
					phase = Phase.ParamName;
					return this;
				default:
					{
						var exception = new SyntaxException(Syntax.Error.InvalidToken);
						exception.content = data.token.ToString();
						throw exception;
					}
				}
			case Phase.ParamName:
				if (Token.Identify == data.token)
				{
					if (null == paramList)
					{
						paramList = new List<KeyValuePair<Lex.Data, Lex.Data>>();
					}
					paramList.Add(new KeyValuePair<Lex.Data, Lex.Data>(paramType_, data));
					paramType_ = null;
					phase = Phase.ParamSeparator;
					return this;
				}
				else
				{
					var exception = new SyntaxException(Syntax.Error.InvalidToken);
					exception.content = data.token.ToString();
					throw exception;
				}
			case Phase.ParamSeparator:
				if (Token.Operator == data.token)
				{
					var op = (Operator)data.number.i_32;
					switch (op)
					{
					case Operator.L_Brackets_End:
						phase = Phase.Code;
						return this;
					case Operator.Comma:
						phase = Phase.ParamType;
						return this;
					default:
						{
							var exception = new SyntaxException(Syntax.Error.InvalidOperator);
							exception.content = op.ToString();
							throw exception;
						}
					}
				}
				else
				{
					var exception = new SyntaxException(Syntax.Error.InvalidToken);
					exception.content = data.token.ToString();
					throw exception;
				}
			case Phase.Code:
				if (Token.Keyword == data.token
					&& Keyword.End == (Keyword)data.number.i_32)
				{
					return parent_;
				}
				break;
			}
			return base.ParseLex(data);
		}
		#endregion override
	}
}

