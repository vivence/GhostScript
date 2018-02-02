
using System.Diagnostics;

namespace Ghost.Script
{
	internal enum BuiltInType
	{
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Float,
		Double,
		Index
	}

	internal class Description_Function
	{
		internal Command[] cmds_;

		public int cmdsCount{get{return cmds_.Length;}}

		internal Description_Function(int pc, int rc, int cc)
		{
			#if DEBUG
			Debug.Assert(0 <= pc && 0 <= rc);
			#endif
			cmds_ = new Command[cc];
		}

		public void SetCMD(int i, Command c)
		{
			#if DEBUG
			Debug.Assert(0 <= i && i < cmds_.Length);
			#endif
			cmds_[i] = c;
		}

		public Command GetCMD(int i)
		{
			#if DEBUG
			Debug.Assert(0 <= i && i < cmds_.Length);
			#endif
			return cmds_[i];
		}
	}

	internal class Description_Class
	{
		internal BuiltInType[] fields_;
		internal Description_Function[] functions_;

		public int fieldsCount{get{return fields_.Length;}}
		public int functionsCount{get{return functions_.Length;}}

		internal Description_Class(int fc, int funcCount)
		{
			#if DEBUG
			Debug.Assert(0 <= fc);
			#endif
			fields_ = new BuiltInType[fc];
			functions_ = new Description_Function[funcCount];
		}

		public void SetField(int i, BuiltInType t)
		{
			#if DEBUG
			Debug.Assert(0 <= i && i < fields_.Length);
			#endif
			fields_[i] = t;
		}

		public BuiltInType GetField(int i)
		{
			#if DEBUG
			Debug.Assert(0 <= i && i < fields_.Length);
			#endif
			return fields_[i];
		}

		public void SetFunction(int i, Description_Function f)
		{
			#if DEBUG
			Debug.Assert(0 <= i && i < functions_.Length);
			#endif
			functions_[i] = f;
		}

		public Description_Function GetFunction(int i)
		{
			#if DEBUG
			Debug.Assert(0 <= i && i < functions_.Length);
			#endif
			return functions_[i];
		}
	}
}
