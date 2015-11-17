using System;
using System.IO;
using System.Text;
using fastJSON;

namespace CommonSerializer.PowerJSON
{
	// This project can output the Class library as a NuGet Package.
	// To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
	public class PowerJsonCommonSerializer : ICommonSerializer
	{
		private readonly SerializationManager _manager;
		private readonly JSONParameters _parameters;

		public PowerJsonCommonSerializer()
			: this(JSON.Manager, JSON.Parameters)
		{
		}

		public PowerJsonCommonSerializer(SerializationManager manager, JSONParameters parameters)
		{
			_manager = manager;
			_parameters = parameters;
		}

		public string Description
		{
			get
			{
				return "PowerJSON, a derivative of fastJSON";
			}
		}

		public string Name
		{
			get
			{
				return "PowerJSON";
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
			return JSON.DeepCopy(t, _parameters, _manager);
		}

		public object Deserialize(string str, Type type)
		{
			return JSON.ToObject(str, type, _parameters, _manager);
		}

		public object Deserialize(TextReader reader, Type type)
		{
			return Deserialize(reader.ReadToEnd(), type);
		}

		public object Deserialize(Stream stream, Type type)
		{
			using (var reader = new StreamReader(stream, Encoding.UTF8, true, 2048, true))
				return Deserialize(reader.ReadToEnd(), type);
		}

		public T Deserialize<T>(TextReader reader)
		{
			return Deserialize<T>(reader.ReadToEnd());

		}

		public T Deserialize<T>(string str)
		{
			return JSON.ToObject<T>(str, _parameters, _manager);
		}

		public T Deserialize<T>(Stream stream)
		{
			return (T)Deserialize(stream, typeof(T));
		}

		public string Serialize<T>(T value)
		{
			return JSON.ToJSON(value, _parameters, _manager);
		}

		public string Serialize(object value, Type type)
		{
			return JSON.ToJSON(value, _parameters, _manager);
		}

		public void Serialize<T>(TextWriter writer, T value)
		{
			writer.Write(JSON.ToJSON(value, _parameters, _manager));
		}

		public void Serialize(TextWriter writer, object value, Type type)
		{
			writer.Write(JSON.ToJSON(value, _parameters, _manager));
		}

		public void Serialize<T>(Stream stream, T value)
		{
			Serialize(stream, value, typeof(T));
		}

		public void Serialize(Stream stream, object value, Type type)
		{
			using (var writer = new StreamWriter(stream, Encoding.UTF8, 2048, true))
				writer.Write(JSON.ToJSON(value, _parameters, _manager));
		}

		public void RegisterSubtype<TBase, TInheritor>(int fieldNumber = -1)
		{
			RegisterSubtype<TBase>(typeof(TInheritor), fieldNumber);
		}

		public void RegisterSubtype<TBase>(Type inheritor, int fieldNumber = -1)
		{
			// no way to turn it off that I can see
		}
	}
}