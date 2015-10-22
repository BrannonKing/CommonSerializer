using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonSerializer.Newtonsoft.Json
{
	internal class JArrayContainer : ISerializedContainer
	{
		public readonly JArray Array;

		public JArrayContainer()
			: this(new JArray())
		{
		}

		public JArrayContainer(JArray array)
		{
			Array = array;
		}

		public int Count
		{
			get { return Array.Count; }
		}

		public bool CanRead
		{
			get { return Array.HasValues; }
		}

		public bool CanWrite
		{
			get { return true; }
		}
	}

	internal class SerializedContainerConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(JArrayContainer) || objectType == typeof(ISerializedContainer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;

			var array = JArray.Load(reader);
			return new JArrayContainer(array);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			((JArrayContainer)value).Array.WriteTo(writer);
		}
	}
}