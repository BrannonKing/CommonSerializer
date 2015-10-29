using System.Collections.Concurrent;
using Jil;
using System.Collections.Generic;
using System;
using System.Collections;

namespace CommonSerializer.Jil
{
	internal class JilSerializedContainer : IEnumerable<string>, ISerializedContainer
	{
		[JilDirective(Ignore = true)]
		internal ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();

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

		public IEnumerator<string> GetEnumerator()
		{
			return Queue.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
