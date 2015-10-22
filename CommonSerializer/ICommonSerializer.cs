﻿using System;
using System.IO;

namespace CommonSerializer
{
	public interface ICommonSerializer
	{
		string Name { get; }
		string Description { get; }
		bool StreamsUtf8 { get; }

		ISerializedContainer GenerateContainer();

		void Serialize<T>(ISerializedContainer container, T value);
		void Serialize<T>(Stream stream, T value);
		void Serialize<T>(TextWriter writer, T value);
		string Serialize<T>(T value);

		void Serialize(ISerializedContainer container, object value, Type type);
		void Serialize(Stream stream, object value, Type type);
		void Serialize(TextWriter writer, object value, Type type);
		string Serialize(object value, Type type);

		T Deserialize<T>(ISerializedContainer container);
		T Deserialize<T>(Stream stream);
		T Deserialize<T>(TextReader reader);
		T Deserialize<T>(string str);

		object Deserialize(ISerializedContainer container, Type type);
		object Deserialize(Stream stream, Type type);
		object Deserialize(TextReader reader, Type type);
		object Deserialize(string str, Type type);

		T DeepClone<T>(T t);
	}
}
