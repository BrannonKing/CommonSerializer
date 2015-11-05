using System;
using System.IO;

namespace CommonSerializer
{
	public interface ICommonSerializer
	{
		string Name { get; }
		string Description { get; }
		bool StreamsUtf8 { get; }

		void Serialize<T>(Stream stream, T value);
		void Serialize<T>(TextWriter writer, T value);
		string Serialize<T>(T value);

		void Serialize(Stream stream, object value, Type type);
		void Serialize(TextWriter writer, object value, Type type);
		string Serialize(object value, Type type);

		T Deserialize<T>(Stream stream);
		T Deserialize<T>(TextReader reader);
		T Deserialize<T>(string str);

		object Deserialize(Stream stream, Type type);
		object Deserialize(TextReader reader, Type type);
		object Deserialize(string str, Type type);

		T DeepClone<T>(T t);
	}

	public interface ICommonSerializerWithContainer: ICommonSerializer
	{
		ISerializedContainer GenerateContainer();

		void Serialize(ISerializedContainer container, object value, Type type);
		void Serialize<T>(ISerializedContainer container, T value);

		T Deserialize<T>(ISerializedContainer container);
		object Deserialize(ISerializedContainer container, Type type);
	}
}
