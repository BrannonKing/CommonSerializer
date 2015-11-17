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
			_options = Options.IncludeInheritedUtc;
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

		public T DeepClone<T>(T t)
		{
			using (var ms = new MemoryStream())
			using (var writer = new StreamWriter(ms))
			{
				JSON.Serialize(t, writer, _options);
				writer.Flush();
				ms.Position = 0;
				using (var reader = new StreamReader(ms))
					return (T)JSON.Deserialize(reader, t.GetType(), _options);
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

		public T Deserialize<T>(TextReader reader)
		{
			return JSON.Deserialize<T>(reader, _options);
		}

		public T Deserialize<T>(string str)
		{
			return JSON.Deserialize<T>(str, _options);
		}

		public T Deserialize<T>(Stream stream)
		{
			using (var reader = new StreamReader(stream, Encoding.UTF8, true, 2048, true))
				return JSON.Deserialize<T>(reader, _options);
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
			JSON.Serialize(value, writer, _options);
		}

		public void Serialize<T>(Stream stream, T value)
		{
			using (var utfWriter = new StreamWriter(stream, Encoding.UTF8, 2048, true))
				Serialize(utfWriter, value);
		}

		public void Serialize(Stream stream, object value, Type type)
		{
			Serialize(stream, value);
		}

		public void RegisterSubtype<TBase, TInheritor>(int fieldNumber = -1)
		{
			if ((_options.GetHashCode() & 0x8) == 0)
				throw new Exception("Inheritance must be specified in the options.");
		}
	}
}