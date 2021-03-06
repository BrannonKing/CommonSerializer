﻿using System;
using System.IO;
using System.Text;
using MsgPack;
using MsgPack.Serialization;

namespace CommonSerializer.MsgPack.Cli
{
	public class MsgPackCommonSerializer: ICommonSerializerWithContainer
	{
		private readonly SerializationContext _context;
		public MsgPackCommonSerializer(SerializationContext context)
		{
			_context = context;
            context.Serializers.RegisterOverride(new MessagePackSerializedContainerSerializer(context));
		}

		public MsgPackCommonSerializer() : this(SerializationContext.Default)
		{
		}

		public string Description
		{
			get
			{
				return "MessagePack for CLI";
			}
		}

		public string Name
		{
			get
			{
				return "MsgPack";
			}
		}

		public bool StreamsUtf8
		{
			get
			{
				return false;
			}
		}

		public T DeepClone<T>(T t)
		{
			var serializer = _context.GetSerializer(t.GetType());
			using(var ms = _streamManager.GetStream("MsgPackClone"))
			{
				serializer.Pack(ms, t);
				ms.Position = 0;
				return (T)serializer.Unpack(ms);
			}
		}

		public object Deserialize(string str, Type type)
		{
			using (var reader = new StringReader(str))
				return Deserialize(reader, type);
		}

		private static readonly Microsoft.IO.RecyclableMemoryStreamManager _streamManager = new Microsoft.IO.RecyclableMemoryStreamManager();

		public object Deserialize(TextReader reader, Type type)
		{
			var line = reader.ReadLine();
			if (line == null)
				return null;
			var bytes = Convert.FromBase64String(line);
			using (var ms = _streamManager.GetStream("MsgPackDeserialize", bytes, 0, bytes.Length))
				return Deserialize(ms, type);
		}

		public object Deserialize(Stream stream, Type type)
		{
			var serializer = _context.GetSerializer(type);
			return serializer.Unpack(stream);
		}

		public object Deserialize(ISerializedContainer container, Type type)
		{
			var psc = container as MsgPackSerializedContainer;
			if (psc == null)
				throw new ArgumentException("Invalid container type. Use the GenerateContainer method.");

			byte[] bytes;
			if (!psc.Queue.TryDequeue(out bytes))
				throw new InvalidDataException("No data available in the container.");

			return _context.GetSerializer(type).UnpackSingleObject(bytes);
		}

		public T Deserialize<T>(Stream stream)
		{
			return (T)Deserialize(stream, typeof(T));
		}

		public T Deserialize<T>(string str)
		{
			return (T)Deserialize(str, typeof(T));
		}

		public T Deserialize<T>(TextReader reader)
		{
			return (T)Deserialize(reader, typeof(T));
		}

		public T Deserialize<T>(ISerializedContainer container)
		{
			return (T)Deserialize(container, typeof(T));
		}

		public ISerializedContainer GenerateContainer()
		{
			return new MsgPackSerializedContainer();
		}

		public string Serialize<T>(T value)
		{
			return Serialize(value, typeof(T));
		}

		public string Serialize(object value, Type type)
		{
			var sb = new StringBuilder();
			using (var stringWriter = new StringWriter(sb))
				Serialize(stringWriter, value, type);
			return sb.ToString();
		}

		public void Serialize<T>(TextWriter writer, T value)
		{
			Serialize(writer, value, typeof(T));
		}

		public void Serialize(TextWriter writer, object value, Type type)
		{
			using (var stream = _streamManager.GetStream("MsgPackSerialize"))
			{
				Serialize(stream, value, type);
				stream.Flush();
				var base64 = Convert.ToBase64String(stream.ToArray());
				writer.Write(base64);
			}
		}

		public void Serialize<T>(Stream stream, T value)
		{
			Serialize(stream, value, typeof(T));
		}

		public void Serialize(Stream stream, object value, Type type)
		{
			var serializer = _context.GetSerializer(type);
			serializer.Pack(stream, value);
		}

		public void Serialize<T>(ISerializedContainer container, T value)
		{
			Serialize(container, value, typeof(T));
		}

        public void Serialize(ISerializedContainer container, object value, Type type)
		{
			var psc = container as MsgPackSerializedContainer;
			if (psc == null)
				throw new ArgumentException("Invalid container type. Use the GenerateContainer method.");

			var bytes = _context.GetSerializer(type).PackSingleObject(value); // TODO: use recyclable memory pool
			psc.Queue.Enqueue(bytes);
		}

		public void RegisterSubtype<TBase, TInheritor>(int fieldNumber = -1)
		{
			RegisterSubtype<TBase>(typeof(TInheritor), fieldNumber);
		}

		public void RegisterSubtype<TBase>(Type inheritor, int fieldNumber = -1)
		{
			_context.GetSerializer<TBase>();
			_context.GetSerializer(inheritor);
			// I don't think it's actually supported
		}
	}
}
