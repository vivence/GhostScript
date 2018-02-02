
using System.Diagnostics;

namespace Ghost.Script
{
	internal class Object
	{
		internal Description_Class description_;
		internal Field[] fields_;

		internal Object(Description_Class d)
		{
			#if DEBUG
			Debug.Assert(null != d);
			#endif
			description_ = d;

			fields_ = new Field[description_.fieldsCount];
		}
	}
} // namespace Ghost.Script
