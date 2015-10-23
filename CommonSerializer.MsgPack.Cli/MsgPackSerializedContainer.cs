using System;
using System.IO;

namespace CommonSerializer.ProtobufNet
{
	internal class MsgPackSerializedContainer : ISerializedContainer
	{
#if DNX451 || NET45
		private static readonly Microsoft.IO.RecyclableMemoryStreamManager _streamManager = new Microsoft.IO.RecyclableMemoryStreamManager();
		private MemoryStream _stream = _streamManager.GetStream("WritableProtobufContainer"); // relying on finalizer for the moment
#else
		private MemoryStream _stream = new MemoryStream();
#endif

		public Stream Stream {  get { return _stream; } }

		[ProtoBuf.ProtoMember(1)]
		public byte[] Data
		{
			get { return _stream.ToArray(); } // need to flush first?
			set
			{
				_stream.Dispose();
#if DNX451 || NET45
				_stream = _streamManager.GetStream("ReadableProtobufContainer", value, 0, value.Length);
#else
				_stream = new MemoryStream(value, false);
#endif
			}
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
