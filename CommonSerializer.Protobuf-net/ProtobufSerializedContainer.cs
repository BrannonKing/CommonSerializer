using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommonSerializer.ProtobufNet
{
	[ProtoBuf.ProtoContract(UseProtoMembersOnly = true)]
	internal class ProtobufSerializedContainer : ISerializedContainer
	{
		internal ConcurrentQueue<byte[]> Queue = new ConcurrentQueue<byte[]>();

		[ProtoBuf.ProtoMember(1)]
		private byte[][] Items
		{
			get { return Queue.ToArray(); }
			set { Queue = new ConcurrentQueue<byte[]>(value); }
		}

		public int Count { get { return Queue.Count; } }

		public bool CanRead
		{
			get
			{
				return !Queue.IsEmpty;
			}
		}

		public bool CanWrite
		{
			get
			{
				return true;
			}
		}
	}
}
