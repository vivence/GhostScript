using System.Runtime.InteropServices;

namespace Ghost.Script
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	internal struct Field
	{
		[FieldOffset(0)]
		public sbyte i_8;
		[FieldOffset(0)]
		public byte ui_8;
		[FieldOffset(0)]
		public short i_16;
		[FieldOffset(0)]
		public ushort ui_16;
		[FieldOffset(0)]
		public int i_32;
		[FieldOffset(0)]
		public uint ui_32;
		[FieldOffset(0)]
		public long i_64;
		[FieldOffset(0)]
		public ulong ui_64;

		[FieldOffset(0)]
		public float f;
		[FieldOffset(0)]
		public double d;

		[FieldOffset(0)]
		public int index;
	}
}

