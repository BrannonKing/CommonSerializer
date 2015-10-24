using MsgPack.Serialization;
using MsgPack;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CommonSerializer.MsgPack.Cli
{
	public class MsgPackSerializedContainer : ISerializedContainer
	{
		internal readonly ConcurrentQueue<byte[]> Queue = new ConcurrentQueue<byte[]>();

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

	internal class MessagePackSerializedContainerSerializer : MessagePackSerializer<ISerializedContainer>
	{
		public MessagePackSerializedContainerSerializer()
			: this(SerializationContext.Default)
		{ }

		public MessagePackSerializedContainerSerializer(SerializationContext context)
			: base(context)
		{
			// If the target objects has complex (non-primitive) objects,
			// you can get serializers which can handle complex type fields.
			// And then, you can cache them to instance fields of this custom serializer.
		}

		protected override void PackToCore(Packer packer, ISerializedContainer objectTree)
		{
			var list = new List<byte[]>();
			byte[] bytes;
			while (((MsgPackSerializedContainer)objectTree).Queue.TryDequeue(out bytes))
				list.Add(bytes);

			packer.PackArrayHeader(list);
			packer.PackArray(list);
		}

		protected override ISerializedContainer UnpackFromCore(Unpacker unpacker)
		{
			var ret = new MsgPackSerializedContainer();
			long arrayLen;
			if (!unpacker.ReadArrayLength(out arrayLen))
				return ret;

			for (int i = 0; i < arrayLen; i++)
			{
				byte[] arr;
				if (!unpacker.ReadBinary(out arr))
					continue;
				ret.Queue.Enqueue(arr);
			}

			return ret;
		}
	}
}
