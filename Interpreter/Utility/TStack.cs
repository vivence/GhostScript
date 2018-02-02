
using System.Diagnostics;
using System;

namespace Ghost.Script
{
	public class TStack<T>
	{
		private T[] data_;
		private int size_;

		#region property
		public int size{get{return size_;}}
		public int capacity
		{
			get
			{
				return data_.Length;
			}
			set
			{
				if (data_.Length == value)
				{
					return;
				}
				#if DEBUG
				Debug.Assert(0 < value);
				#endif
				ResetCapacity(value, Math.Min(size_, value));
			}
		}
		#endregion property

		public TStack(int capacity = 4)
		{
			#if DEBUG
			Debug.Assert(0 < capacity);
			#endif
			data_ = MemeryPool.CreateArray<T>(capacity);
			size_ = 0;
		}

		public void Push(T v)
		{
			if (size_ < data_.Length)
			{
				data_[size_++] = v;
			}
			else
			{
				ResetCapacity(data_.Length * 2, size_);
			}
		}

		public T Pop()
		{
			#if DEBUG
			Debug.Assert(0 < size_);
			#endif
			var v = data_[size_-1];
			--size_;
			return v;
		}

		public T Peek()
		{
			#if DEBUG
			Debug.Assert(0 < size_);
			#endif
			return data_[size_-1];
		}

		private void ResetCapacity(int capacity, int dataLen)
		{
			var newData = MemeryPool.CreateArray<T>(capacity);
			for (int i = 0; i < dataLen; ++i)
			{
				newData[i] = data_[i];
			}
			MemeryPool.DestroyArray(data_);
			data_ = newData;
		}
	}
} // namespace Ghost.Script
