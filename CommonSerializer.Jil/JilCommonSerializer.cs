using Jil;
using System;
using System.IO;
using System.Text;

namespace CommonSerializer.Jil
{
	public class JilCommonSerializer : ICommonSerializer
	{
		private readonly Options _options;

		public JilCommonSerializer()
		{
			_options = Options.IncludeInherited;
		}

		public JilCommonSerializer(Options options)
		{
			_options = options;
		}

		public string Description
		{
			get
			{
				return "K. Montrose's Jil Serializer";
			}
		}

		public string Name
		{
			get
			{
				return "Jil";
			}
		}

		public bool StreamsUtf8
		{
			get
			{
				return true;
			}
		}

#if DNX451 || NET45
		private static readonly Microsoft.IO.RecyclableMemoryStreamManager _streamManager = new Microsoft.IO.RecyclableMemoryStreamManager();
#endif

		public T DeepClone<T>(T t)
		{
#if DNX451 || NET45
			using (var ms = _streamManager.GetStream("Clone"))
#else
			using(var ms = new MemoryStream())
#endif
			{
				Serialize(ms, t);
				ms.Position = 0;
				return (T)Deserialize(ms, t.GetType());

				// alternate that will use less memory and might be faster (not tested yet):
				//using (var writer = new BsonWriter(ms))
				//	_serializer.Serialize(writer, t);
				//ms.Position = 0;
				//using (var reader = new BsonReader(ms))
				//	return _serializer.Deserialize<T>(reader);
			}
		}

		public object Deserialize(string str, Type type)
		{
			using (var reader = new StringReader(str))
				return Deserialize(reader, type);
		}

		public object Deserialize(TextReader reader, Type type)
		{
			return JSON.Deserialize(reader, type, _options);
		}

		public object Deserialize(Stream stream, Type type)
		{
			using (var utfReader = new StreamReader(stream, Encoding.UTF8, true, 2048, true))
				return Deserialize(utfReader, type);
		}

		public object Deserialize(ISerializedContainer container, Type type)
		{
			var jTokenContainer = container as JilSerializedContainer;
			if (jTokenContainer == null)
				throw new ArgumentException("Invalid container. Use the GenerateContainer method.");

			byte[] bytes;
			if (!jTokenContainer.Queue.TryDequeue(out bytes))
				throw new InvalidDataException("No data available in the container.");

#if DNX451 || NET45
			using (var ms = _streamManager.GetStream("DeserializeContainer", bytes, 0, bytes.Length))
#else
			using (var ms = new MemoryStream(bytes, false))
#endif
				return Deserialize(ms, type);
		}

		public T Deserialize<T>(TextReader reader)
		{
			return (T)Deserialize(reader, typeof(T));
		}

		public T Deserialize<T>(string str)
		{
			return (T)Deserialize(str, typeof(T));
		}

		public T Deserialize<T>(Stream stream)
		{
			return (T)Deserialize(stream, typeof(T));
		}

		public T Deserialize<T>(ISerializedContainer container)
		{
			return (T)Deserialize(container, typeof(T));
		}

		public ISerializedContainer GenerateContainer()
		{
			return new JilSerializedContainer();
		}

		public string Serialize<T>(T value)
		{
			return JSON.Serialize(value, _options);
		}

		public string Serialize(object value, Type type)
		{
			return JSON.Serialize(value, _options);
		}

		public void Serialize<T>(TextWriter writer, T value)
		{
			JSON.Serialize(value, writer, _options);
		}

		public void Serialize(TextWriter writer, object value, Type type)
		{
			JSON.Serialize(value, writer, _options); // no place to use type with this serializer
		}

		public void Serialize<T>(Stream stream, T value)
		{
			using (var utfWriter = new StreamWriter(stream, Encoding.UTF8, 2048, true))
				Serialize(utfWriter, value);
		}

		public void Serialize(Stream stream, object value, Type type)
		{
			using (var utfWriter = new StreamWriter(stream, Encoding.UTF8, 2048, true))
				Serialize(utfWriter, value, type);
		}

		public void Serialize<T>(ISerializedContainer container, T value)
		{
			Serialize(container, value, typeof(T));
		}

		public void Serialize(ISerializedContainer container, object value, Type type)
		{
			var jTokenContainer = container as JilSerializedContainer;
			if (jTokenContainer == null)
				throw new ArgumentException("Invalid container. Use the GenerateContainer method.");

#if DNX451 || NET45
			using (var stream = _streamManager.GetStream("SerializeContainer"))
#else
			using (var stream = new MemoryStream())
#endif
			{
				Serialize(stream, value, type);
				jTokenContainer.Queue.Enqueue(stream.ToArray());
			}
		}
	}
}