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
				_serializer.DeserializeDirect<T>(ms, out var ret);
				return ret;
			}
		}

		public T Deserialize<T>(Stream stream)
		{
			return (T)_serializer.Deserialize(stream);
		}

		public T Deserialize<T>(TextReader reader)
		{
			var text = reader.ReadToEnd(); // if there were multiple objects in a single stream, we wouldn't want to do it this way
			using(var stream = new StringReader(text))
			_serializer.Deserialize()
		}

		public T Deserialize<T>(string str)
		{
			throw new NotImplementedException();
		}

		public object Deserialize(Stream stream, Type type)
		{

			throw new NotImplementedException();
		}

		public object Deserialize(TextReader reader, Type type)
		{
			throw new NotImplementedException();
		}

		public object Deserialize(string str, Type type)
		{
			throw new NotImplementedException();
		}

		public void RegisterSubtype<TBase, TInheritor>(int fieldNumber = -1)
		{
			_serializer.AddTypes(new[] { typeof(TBase), typeof(TInheritor) });
		}

		public void RegisterSubtype<TBase>(Type inheritor, int fieldNumber = -1)
		{
			_serializer.AddTypes(new[] { typeof(TBase), inheritor });
		}

		public void Serialize<T>(Stream stream, T value)
		{
			_serializer.Serialize(stream, value);
		}

		public void Serialize<T>(TextWriter writer, T value)
		{
			throw new NotImplementedException();
		}

		public string Serialize<T>(T value)
		{
			throw new NotImplementedException();
		}

		public void Serialize(Stream stream, object value, Type type)
		{
			throw new NotImplementedException();
		}

		public void Serialize(TextWriter writer, object value, Type type)
		{
			throw new NotImplementedException();
		}

		public string Serialize(object value, Type type)
		{
			throw new NotImplementedException();
		}
	}
}