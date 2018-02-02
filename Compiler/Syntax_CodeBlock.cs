using System.Text;

namespace Ghost.Script
{
	internal abstract class Syntax_CodeBlock : Syntax_Node
	{
		// TODO save parsed code
		// 数据：当前代码块栈（索引），外部引用，值

		public enum CodePhase
		{
			Code,
			Return,
			ReturnSeparator,
		}
		public CodePhase codePhase{get;internal set;}

		#region override
		internal override void DoConstruct(Lex.Data data)
		{
			base.DoConstruct(data);
			codePhase = CodePhase.Code;
		}

		internal override Syntax_Node ParseLex(Lex.Data data)
		{
			switch (codePhase)
			{
			case CodePhase.Code:
				// TODO parse code
				switch (data.token)
				{
				case Token.Keyword:
					{
						var keyword = (Keyword)data.number.i_32;
						switch (keyword)
						{
						case Keyword.If:
							return AddChild(Syntax_Node.Create<Syntax_If>(data, this));
						case Keyword.Loop:
							return AddChild(Syntax_Node.Create<Syntax_Loop>(data, this));
						case Keyword.Block:
							return AddChild(Syntax_Node.Create<Syntax_Block>(data, this));
						case Keyword.Return:
							codePhase = CodePhase.Return;
							return this;
						default:
//							{
//								var exception = new SyntaxException(Syntax.Error.InvalidKeyword);
//								exception.content = keyword.ToString();
//								throw exception;
//							}
							return this;
						}
					}
				default:
//					{
//						var exception = new SyntaxException(Syntax.Error.InvalidToken);
//						exception.content = data.token.ToString();
//						throw exception;
//					}
					return this;
				}
			case CodePhase.Return:
				switch (data.token)
				{
				case Token.Keyword:
					// TODO save
					if (Syntax.constantKeywordList.Contains((Keyword)data.number.i_32))
					{
						// TODO save
						codePhase = CodePhase.ReturnSeparator;
						return this;
					}
					else
					{
						var exception = new SyntaxException(Syntax.Error.InvalidToken);
						exception.content = data.token.ToString();
						throw exception;
					}
				case Token.Identify:
					// TODO save
					codePhase = CodePhase.ReturnSeparator;
					return this;
				default:
					if (Syntax.constantTokenList.Contains(data.token))
					{
						// TODO save
						codePhase = CodePhase.ReturnSeparator;
						return this;
					}
					else
					{
						var exception = new SyntaxException(Syntax.Error.InvalidToken);
						exception.content = data.token.ToString();
						throw exception;
					}
				}
			case CodePhase.ReturnSeparator:
				if (Token.Operator == data.token)
				{
					var op = (Operator)data.number.i_32;
					if (Operator.Comma == op)
					{
						codePhase = CodePhase.Return;
						return this;
					}
					else
					{
						var exception = new SyntaxException(Syntax.Error.InvalidOperator);
						exception.content = op.ToString();
						throw exception;
					}
				}
				else
				{
					var exception = new SyntaxException(Syntax.Error.InvalidToken);
					exception.content = data.token.ToString();
					throw exception;
				}
			}
			return this;
		}
		#endregion override
	}
}

