using System;

namespace Ghost.Script
{
	internal class Syntax_Loop : Syntax_CodeBlock
	{
		public enum Phase
		{
			None,
			Condition,
			Code
		}

		public Phase phase{get;internal set;}
		public bool tailCondition{get;internal set;}

		#region override
		internal override void DoConstruct(Lex.Data data)
		{
			base.DoConstruct(data);
			phase = Phase.None;
			tailCondition = false;
		}

		internal override void DoDeconstruct()
		{
			base.DoDeconstruct();
		}

		internal override Syntax_Node ParseLex(Lex.Data data)
		{
			switch (phase)
			{
			case Phase.None:
				if (Token.Operator == data.token 
					&& Operator.S_Brackets_Beg == (Operator)data.number.i_32)
				{
					phase = Phase.Condition;
					tailCondition = false;
					return this;
				}
				else
				{
					phase = Phase.Code;
					tailCondition = true;
					return ParseLex(data);
				}
			case Phase.Condition:
				if (Token.Operator == data.token 
					&& Operator.S_Brackets_End == (Operator)data.number.i_32)
				{
					if (tailCondition)
					{
						return parent_;
					}
					else
					{
						phase = Phase.Code;
						return this;
					}
				}
				// TODO
				break;
			case Phase.Code:
				if (Token.Keyword == data.token)
				{
					var keyword = (Keyword)data.number.i_32;
					if (tailCondition)
					{
						if (Keyword.Until == keyword)
						{
							phase = Phase.Condition;
							return this;
						}
					}
					else
					{
						if (Keyword.End == keyword)
						{
							return parent_;
						}
					}
				}
				break;
			}
			return base.ParseLex(data);
		}
		#endregion override
	}
}

