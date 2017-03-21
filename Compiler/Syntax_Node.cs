using System.Collections.Generic;
using System.Text;

namespace Ghost.Script
{
	internal abstract class Syntax_Node
	{
		internal Syntax_Node parent_;
		internal List<Syntax_Node> children_;

		public Syntax_Node ()
		{
			
		}
			
		public override string ToString ()
		{
			var sb = new StringBuilder();
			ToString(sb);
			return sb.ToString();
		}
		internal void ToString(StringBuilder sb)
		{
			if (null != parent_)
			{
				parent_.ToString(sb);
			}
			DoToString(sb);
		}
		internal virtual void DoToString(StringBuilder sb)
		{
			sb.AppendFormat(".{0}", GetType().Name);
		}

		internal void Construct(Lex.Data data, Syntax_Node p)
		{
			parent_ = p;
			DoConstruct(data);
		}

		internal void Deconstruct()
		{
			DoDeconstruct();
			parent_ = null;
			if (null != children_)
			{
				children_.Clear();
			}
		}

		internal virtual void DoConstruct(Lex.Data data){}
		internal virtual void DoDeconstruct(){}

		internal abstract Syntax_Node ParseLex(Lex.Data data);

		public static T Create<T>() where T:Syntax_Node,new()
		{
			// TODO reuse
			var node = new T();
			node.Construct(null, null);
			return node;
		}

		public static T Create<T>(Syntax_Node parent) where T:Syntax_Node,new()
		{
			// TODO reuse
			var node = new T();
			node.Construct(null, parent);
			return node;
		}

		public static T Create<T>(Lex.Data data, Syntax_Node parent) where T:Syntax_Node,new()
		{
			// TODO reuse
			var node = new T();
			node.Construct(data, parent);
			return node;
		}

		public static void Destroy<T>(T p) where T:Syntax_Node
		{
			p.Deconstruct();
			// TODO recycle
		}
	}
}

