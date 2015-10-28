using System.Collections.Concurrent;
using Jil;

namespace CommonSerializer.Jil
{
	internal class JilSerializedContainer : ISerializedContainer
	{
		[JilDirective(Ignore = true)]
		internal ConcurrentQueue<byte[]> Queue = new ConcurrentQueue<byte[]>();

		[JilDirective]
		private byte[][] Items
		{
			get { return Queue.ToArray(); }
			set { Queue = new ConcurrentQueue<byte[]>(value); }
		}

		[JilDirective(Ignore = true)]
		public int Count { get { return Queue.Count; } }

		[JilDirective(Ignore = true)]
		public bool CanRead
		{
			get
			{
				return !Queue.IsEmpty;
			}
		}

		[JilDirective(Ignore = true)]
		public bool CanWrite
		{
			get
			{
				return true;
			}
		}
	}
}
