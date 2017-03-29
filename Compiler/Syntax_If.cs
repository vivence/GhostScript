using System.Text;

namespace Ghost.Script
{
	internal class Syntax_If : Syntax_CodeBlock
	{
		public enum Phase
		{
			ConditionBegin,
			Condition,
			Code,
			LastCode,
		}

		public Phase phase{get;internal set;}

		#region override
		internal override void DoConstruct(Lex.Data data)
		{
			base.DoConstruct(data);
			phase = Phase.ConditionBegin;
		}

		internal override void DoDeconstruct()
		{
			base.DoDeconstruct();
		}

		internal override Syntax_Node ParseLex(Lex.Data data)
		{
			switch (phase)
			{
			case Phase.ConditionBegin:
				if (Token.Operator == data.token 
					&& Operator.S_Brackets_Beg == (Operator)data.number.i_32)
				{
					phase = Phase.Condition;
					return this;
				}
				else
				{
					var exception = new SyntaxException(Syntax.Error.InvalidFunction);
					exception.content = "Need\"(\"";
					throw exception;
				}
			case Phase.Condition:
				if (Token.Operator == data.token 
					&& Operator.S_Brackets_End == (Operator)data.number.i_32)
				{
					phase = Phase.Code;
					return this;
				}
				// TODO
				break;
			case Phase.Code:
				if (Token.Keyword == data.token)
				{
					var keyword = (Keyword)data.number.i_32;
					switch (keyword)
					{
					case Keyword.Elseif:
						phase = Phase.ConditionBegin;
						return this;
					case Keyword.Else:
						phase = Phase.LastCode;
						return this;
					case Keyword.End:
						return parent_;
					}
				}
				break;
			case Phase.LastCode:
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

