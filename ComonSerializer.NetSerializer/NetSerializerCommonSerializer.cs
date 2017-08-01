using System;
using System.IO;
using System.Text;
using NetSerializer;

namespace CommonSerializer.Jil
{
	public class NetSerializerCommonSerializer : ICommonSerializer
	{
		private readonly Serializer _serializer;

		public NetSerializerCommonSerializer(bool supportCallbacks = false)
		{
			var settings = new Settings { SupportSerializationCallbacks = supportCallbacks, SupportIDeserializationCallback = supportCallbacks };
			_serializer = new Serializer(new Type[0], settings);
		}

		public string Description
		{
			get
			{
				return "Tomi Valkeinen's NetSerializer";
			}
		}

		public string Name
		{
			get
			{
				return "NetSerializer";
			}
		}

        public bool StreamsUtf8
        {
            get
            {
                return true;
            }
        }

        public T DeepClone<T>(T t)
        {
            using (var ms = new MemoryStream())
            {
                _serializer.SerializeDirect(ms, t);
                ms.Position = 0;
                _serializer.DeserializeDirect<T>(ms, out var value);
                return value;
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            _serializer.DeserializeDirect<T>(stream, out var value);
            return value;
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

        public T Deserialize<T>(string str)
        {
            using (var reader = new StringReader(str))
                return (T)Deserialize(reader, typeof(T));
        }

        public object Deserialize(Stream stream, Type type)
        {
            return _serializer.Deserialize(stream); // TODO: convert to target type?
        }

        public object Deserialize(string str, Type type)
        {
            using (var reader = new StringReader(str))
                return Deserialize(reader, type);
        }

        public T Deserialize<T>(TextReader reader)
        {
            return (T)Deserialize(reader, typeof(T));
        }

        public void RegisterSubtype<TBase, TInheritor>(int fieldNumber = -1)
        {
        }

        public void RegisterSubtype<TBase>(Type inheritor, int fieldNumber = -1)
        {
        }

        public void Serialize<T>(Stream stream, T value)
        {
            _serializer.Serialize(stream, value);
        }

        public void Serialize<T>(TextWriter writer, T value)
        {
            Serialize(writer, value, typeof(T));
        }

        public string Serialize<T>(T value)
        {
            return Serialize(value, typeof(T));
        }

        public void Serialize(Stream stream, object value, Type type)
        {
            _serializer.Serialize(stream, value);
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

        public string Serialize(object value, Type type)
        {
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
                Serialize(stringWriter, value, type);
            return sb.ToString();
        }
    }
}