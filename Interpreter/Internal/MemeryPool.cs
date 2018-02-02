
namespace Ghost.Script
{
	internal static class MemeryPool
	{
		public static void DestroyArray<T>(T[] array)
		{

		}

		public static T[] CreateArray<T>(int size)
		{
			return new T[size];
		}
	}
} // namespace Ghost.Script
