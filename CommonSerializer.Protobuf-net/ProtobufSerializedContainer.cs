using System;
using System.IO;

namespace CommonSerializer.ProtobufNet
{
	[ProtoBuf.ProtoContract(UseProtoMembersOnly = true)]
	internal class ProtobufSerializedContainer : ISerializedContainer
	{
		private MemoryStream _stream = new MemoryStream();

		public Stream Stream {  get { return _stream; } }

		[ProtoBuf.ProtoMember(1)]
		public byte[] Data
		{
			get { return _stream.ToArray(); } // need to flush first?
			set { _stream = new MemoryStream(value, false); }
		}

		[ProtoBuf.ProtoMember(2)]
		public int Count { get; internal set; }

		public bool CanRead
		{
			get
			{
				return _stream.Position < _stream.Length;
			}
		}

		public bool CanWrite
		{
			get
			{
				return _stream.Position >= _stream.Length;
			}
		}
	}
}
