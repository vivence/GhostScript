using System;

namespace Ghost.Script
{
	internal class Syntax_Block : Syntax_CodeBlock
	{
		#region override
		internal override void DoConstruct(Lex.Data data)
		{
			
		}

		internal override void DoDeconstruct()
		{

		}

		internal override Syntax_Node ParseLex(Lex.Data data)
		{
			if (Token.Keyword == data.token
				&& Keyword.End == (Keyword)data.number.i_32)
			{
				return parent_;
			}
			return base.ParseLex(data);
		}
		#endregion override
	}
}

