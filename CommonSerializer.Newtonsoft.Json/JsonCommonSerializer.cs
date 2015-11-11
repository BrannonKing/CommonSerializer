using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;

namespace CommonSerializer.Newtonsoft.Json
{
	// This project can output the Class library as a NuGet Package.
	// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
	public class JsonCommonSerializer : ICommonSerializerWithContainer
	{
		private readonly JsonSerializer _serializer;

		public JsonCommonSerializer(bool indented = false)
			: this(new JsonSerializer
			{
				TypeNameHandling = TypeNameHandling.Auto,
				Formatting = indented ? Formatting.Indented : Formatting.None,
			})
		{
		}

		public JsonCommonSerializer(JsonSerializer serializer)
		{
			_serializer = serializer;
			_serializer.Converters.Add(new SerializedContainerConverter());
		}

		public string Description
		{
			get
			{
				return "Newtonsoft Json.NET";
			}
		}

		public string Name
		{
			get
			{
				return "Json.NET";
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

		public void RegisterSubtype<TBase, TInheritor>(int fieldNumber = -1)
		{
			var contract = _serializer.ContractResolver.ResolveContract(typeof(TBase)) as JsonContainerContract;
			if (contract != null && contract.ItemTypeNameHandling == TypeNameHandling.None && _serializer.TypeNameHandling != TypeNameHandling.Auto)
				contract.ItemTypeNameHandling = TypeNameHandling.Auto;
		}

		public object Deserialize(string str, Type type)
		{
			using (var reader = new StringReader(str))
				return Deserialize(reader, type);
		}

		public object Deserialize(TextReader reader, Type type)
		{
			using (var jsonReader = new JsonTextReader(reader) { CloseInput = false })
				return _serializer.Deserialize(jsonReader, type);
		}

		public object Deserialize(Stream stream, Type type)
		{
			using (var utfReader = new StreamReader(stream, Encoding.UTF8, true, 2048, true))
			using (var reader = new JsonTextReader(utfReader) { CloseInput = false })
				return _serializer.Deserialize(reader, type);
		}

		public object Deserialize(ISerializedContainer container, Type type)
		{
			var jTokenContainer = container as JArrayContainer;
			if (jTokenContainer == null)
				throw new ArgumentException("Invalid container. Use the GenerateContainer method.");

			if (jTokenContainer.Array.HasValues)
			{
				var first = jTokenContainer.Array.First;
				first.Remove();
				using (var reader = first.CreateReader())
					return _serializer.Deserialize(reader, type);
			}
			return null;
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
			return new JArrayContainer();
		}

		public string Serialize<T>(T value)
		{
			return Serialize(value, typeof(T));
		}

		public string Serialize(object value, Type type)
		{
			var sb = new StringBuilder();
			using (var stringWriter = new StringWriter(sb))
			using (var writer = new JsonTextWriter(stringWriter) { CloseOutput = false })
				_serializer.Serialize(writer, value, type);

			return sb.ToString();
		}

		public void Serialize<T>(TextWriter writer, T value)
		{
			Serialize(writer, value, typeof(T));
		}

		public void Serialize(TextWriter writer, object value, Type type)
		{
			using (var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false })
				_serializer.Serialize(jsonWriter, value, type);
		}

		public void Serialize<T>(Stream stream, T value)
		{
			Serialize(stream, value, typeof(T));
		}

		public void Serialize(Stream stream, object value, Type type)
		{
			using (var utfWriter = new StreamWriter(stream, Encoding.UTF8, 2048, true))
			using (var jsonWriter = new JsonTextWriter(utfWriter) { CloseOutput = false })
				_serializer.Serialize(jsonWriter, value, type);
		}

		public void Serialize<T>(ISerializedContainer container, T value)
		{
			Serialize(container, value, typeof(T));
		}

		public void Serialize(ISerializedContainer container, object value, Type type)
		{
			var jTokenContainer = container as JArrayContainer;
			if (jTokenContainer == null)
				throw new ArgumentException("Invalid container. Use the GenerateContainer method.");

			using (var writer = jTokenContainer.Array.CreateWriter())
				_serializer.Serialize(writer, value, type);
		}
	}
}