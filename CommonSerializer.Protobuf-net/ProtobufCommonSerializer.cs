using System;
using System.IO;
using System.Reflection;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;

namespace CommonSerializer.ProtobufNet
{
	public class ProtobufCommonSerializer: ICommonSerializer
	{
		private readonly RuntimeTypeModel _runtime;

		public ProtobufCommonSerializer(MethodInfo classFactory = null)
		{
			_runtime = TypeModel.Create();
			if (classFactory != null)
				_runtime.SetDefaultFactory(classFactory);
			RegisterSubtype<ISerializedContainer, ProtobufSerializedContainer>(1);
		}

		public ProtobufCommonSerializer(RuntimeTypeModel runtime)
		{
			_runtime = runtime;
		}

		public string Description
		{
			get
			{
				return "M. Gravell's Protocol Buffers Implementation";
			}
		}

		public string Name
		{
			get
			{
				return "Protobuf-net";
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
			return (T)_runtime.DeepClone(t);
		}

		public object Deserialize(string str, Type type)
		{
			using (var reader = new StringReader(str))
				return Deserialize(reader, type);
		}

		public object Deserialize(TextReader reader, Type type)
		{
			var line = reader.ReadLine();
			if (line == null)
				return null;
			var bytes = Convert.FromBase64String(line);
			using (var ms = new MemoryStream(bytes, false))
				return Deserialize(ms, type);
		}

		public object Deserialize(Stream stream, Type type)
		{
			return _runtime.DeserializeWithLengthPrefix(stream, null, type, PrefixStyle.Fixed32, 0);
		}

		public object Deserialize(ISerializedContainer container, Type type)
		{
			var psc = container as ProtobufSerializedContainer;
			if (psc == null)
				throw new ArgumentException("Invalid container type. Use the GenerateContainer method.");

			return Deserialize(psc.Stream, type);
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
			return new ProtobufSerializedContainer();
		}

		public void RegisterSubtype<TBase, TInheritor>(int fieldNumber)
		{
			_runtime[typeof(TBase)].AddSubType(fieldNumber, typeof(TInheritor));
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
			using (var stream = new MemoryStream())
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
			_runtime.SerializeWithLengthPrefix(stream, value, type, PrefixStyle.Fixed32, 0);
		}

		public void Serialize<T>(ISerializedContainer container, T value)
		{
			Serialize(container, value, typeof(T));
		}

        public void Serialize(ISerializedContainer container, object value, Type type)
		{
			var psc = container as ProtobufSerializedContainer;
			if (psc == null)
				throw new ArgumentException("Invalid container type. Use the GenerateContainer method.");

			Serialize(psc.Stream, value, type);
			psc.Count++;
		}
    }
}
