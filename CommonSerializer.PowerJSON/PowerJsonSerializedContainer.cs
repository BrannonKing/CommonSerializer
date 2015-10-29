using System;
using fastJSON;
using System.Collections.Concurrent;

namespace CommonSerializer.Newtonsoft.Json
{
	internal class PowerJsonSerializedContainer : ISerializedContainer
	{
		internal readonly ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();

		public int Count
		{
			get { return Queue.Count; }
		}

		public bool CanRead
		{
			get { return !Queue.IsEmpty; }
		}

		public bool CanWrite
		{
			get { return true; }
		}
	}

	internal class SerializedContainerConverter : JsonConverter<ISerializedContainer, string[]> // second one gets written
	{
		protected override string[] Convert(string fieldName, ISerializedContainer fieldValue)
		{
			if (fieldValue is PowerJsonSerializedContainer)
				return ((PowerJsonSerializedContainer)fieldValue).Queue.ToArray();
			return null;
		}

		protected override ISerializedContainer Revert(string fieldName, string[] fieldValue)
		{
			var ret = new PowerJsonSerializedContainer();
			if (fieldValue != null)
			{
				foreach (var value in fieldValue)
					ret.Queue.Enqueue(value);
			}
			return ret;
		}
	}
}