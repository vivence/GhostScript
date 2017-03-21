using System.Text;

namespace Ghost.Script
{
	internal class Syntax_File : Syntax_Node
	{
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
				if (Keyword.Interface == (Keyword)data.number.i_32)
				{
					return Syntax_Node.Create<Syntax_Interface>(data, this);
				}
				else if (Keyword.Class == (Keyword)data.number.i_32)
				{
					return Syntax_Node.Create<Syntax_Class>(data, this);
				}
				else
				{
					// TODO invalid keyword
				}
				break;
			case Token.Identify:
				return Syntax_Node.Create<Syntax_Function>(data, this);
			default:
				// TODO invalid token
				break;
			}
			return parent_;
		}
		#endregion override
	}
}

