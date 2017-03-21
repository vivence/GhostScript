using System.Text;

namespace Ghost.Script
{
	internal class Syntax_Interface : Syntax_Node
	{
		public string name{get;internal set;}

		#region override
		internal override void DoToString(StringBuilder sb)
		{
			sb.AppendFormat(".{0}(Interface)", name);
		}

		internal override void DoConstruct(Lex.Data data)
		{

		}

		internal override void DoDeconstruct()
		{

		}

		internal override Syntax_Node ParseLex(Lex.Data data)
		{
			return this;
		}
		#endregion override
	}
}

