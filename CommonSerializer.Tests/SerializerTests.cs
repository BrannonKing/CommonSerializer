using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using CommonSerializer.Jil;
using CommonSerializer.MsgPack.Cli;
using CommonSerializer.Newtonsoft.Json;
using CommonSerializer.PowerJSON;
using CommonSerializer.ProtobufNet;
using Xunit;

namespace CommonSerializer.Tests
{
	public class SerializerTests
	{
		private IEnumerable<ICommonSerializer> Serializers
		{
			get
			{
				yield return new JsonCommonSerializer();
				yield return new ProtobufCommonSerializer();
				//yield return new JilCommonSerializer(); I guess .net 4.7 doesn't actually support netstandard 1.6
				yield return new MsgPackCommonSerializer();
				yield return new PowerJsonCommonSerializer();
			}
		}

		[Fact]
		public void RoundTrip()
		{
			var data = new TestData
			{
				TestBool = true,
				TestByteArray = new byte[] { 0x00, 0x02, 0x04, 0x05, 0x01 },
				TestDouble = 7.0,
				TestByte = 0xff,
				TestDateTime = new DateTime(2089, 9, 27),
				TestInt = 7,
				TestList = new List<int> { 4, 55, 4, 6, 7 },
				TestLong = 777,
				TestShort = 456,
				TestString = "Hello World!",
				TestChar = 'R',
				TestDecimal = 100,
				TestsByte = 0x05,
				TestuInt = 80,
				DontGo = 42,
				Children = new List<SubTestData> { new SubTestData { Name = "one" }, new SubTestData { Name = "two" } }
			};

			foreach (var serializer in Serializers)
			{
				using (var stream = new MemoryStream())
				{
					serializer.Serialize(stream, data);
					stream.Position = 0;
					var result = serializer.Deserialize<TestData>(stream);
					VerifyEqual(data, result);
				}

				using (var stream = new MemoryStream())
				using (var writer = new StreamWriter(stream))
				{
					serializer.Serialize(writer, data);
					writer.Flush();
					stream.Position = 0;
					using (var reader = new StreamReader(stream))
					{
						var result = serializer.Deserialize<TestData>(reader);
						VerifyEqual(data, result);
					}
				}

				var str = serializer.Serialize(data);
				var result2 = serializer.Deserialize<TestData>(str);
				VerifyEqual(data, result2);

				var clone = serializer.DeepClone(data);
				VerifyEqual(data, clone);
			}
		}

		[Fact]
		public void RoundTripWithType()
		{
			var data = new TestData
			{
				TestBool = true,
				TestByteArray = new byte[] { 0x00, 0x02, 0x04, 0x05, 0x01 },
				TestDouble = 7.0,
				TestByte = 0xff,
				TestDateTime = new DateTime(2089, 9, 27),
				TestInt = 7,
				TestList = new List<int> { 4, 55, 4, 6, 7 },
				TestLong = 777,
				TestShort = 456,
				TestString = "Hello World!",
				TestChar = 'R',
				TestDecimal = 100,
				TestsByte = 0x05,
				TestuInt = 80,
				DontGo = 42,
				Children = new List<SubTestData> { new SubTestData { Name = "one" }, new SubTestData { Name = "two" } }
			};

			foreach (var serializer in Serializers)
			{
				using (var stream = new MemoryStream())
				{
					serializer.Serialize(stream, data, data.GetType());
					stream.Position = 0;
					var result = (TestData)serializer.Deserialize(stream, data.GetType());
					VerifyEqual(data, result);
				}

				using (var stream = new MemoryStream())
				using (var writer = new StreamWriter(stream))
				{
					serializer.Serialize(writer, data, data.GetType());
					writer.Flush();
					stream.Position = 0;
					using (var reader = new StreamReader(stream))
					{
						var result = (TestData)serializer.Deserialize(reader, typeof(TestData));
						VerifyEqual(data, result);
					}
				}

				var str = serializer.Serialize(data, typeof(TestData));
				var result2 = (TestData)serializer.Deserialize(str, typeof(TestData));
				VerifyEqual(data, result2);

				var clone = serializer.DeepClone(data);
				VerifyEqual(data, clone);
			}
		}

		private void VerifyEqual(TestData data, TestData result)
		{
			Assert.Equal(data.Children, result.Children);
			Assert.NotEqual(data.DontGo, result.DontGo);
			Assert.Equal(data.TestBool, result.TestBool);
			Assert.Equal(data.TestByte, result.TestByte);
			Assert.Equal(data.TestByteArray, result.TestByteArray);
			Assert.Equal(data.TestChar, result.TestChar);
			Assert.Equal(data.TestDateTime, result.TestDateTime);
			Assert.Equal(data.TestDecimal, result.TestDecimal);
			Assert.Equal(data.TestDouble, result.TestDouble);
			Assert.Equal(data.TestInt, result.TestInt);
			Assert.Equal(data.TestList, result.TestList);
			Assert.Equal(data.TestLong, result.TestLong);
			Assert.Equal(data.TestShort, result.TestShort);
			Assert.Equal(data.TestString, result.TestString);
			Assert.Equal(data.TestuInt, result.TestuInt);
		}

		[Fact]
		public void TestPartialCreation()
		{
			foreach (var serializer in Serializers.OfType<ICommonSerializerWithContainer>())
			{
				var container = serializer.GenerateContainer();
				Assert.True(container.CanWrite);

				serializer.Serialize(container, "howdy");
				serializer.Serialize(container, 42);
				serializer.Serialize(container, 101.7);
				var st = new SubTestData { Name = "nm1" };
				serializer.Serialize(container, st);

				Assert.Equal(4, container.Count);
				Assert.True(container.CanWrite);

				var wrapper = new ContainerWrapper { Container = container };

				ContainerWrapper wrapper2;
				using(var ms = new MemoryStream())
				{
					serializer.Serialize(ms, wrapper);
					ms.Position = 0;
					wrapper2 = serializer.Deserialize<ContainerWrapper>(ms);
				}

				container = wrapper2.Container;
				Assert.True(container.CanRead);
				Assert.Equal("howdy", serializer.Deserialize<string>(container));
				Assert.Equal(42, serializer.Deserialize<int>(container));
				Assert.Equal(101.7, serializer.Deserialize<double>(container));
				Assert.Equal(st, serializer.Deserialize<SubTestData>(container));

				Assert.False(container.CanRead);
			}
		}
	}

	[DataContract]
	public class TestData
	{
		[DataMember(Order = 1)]
		public bool TestBool { get; set; }

		[DataMember(Order = 2)]
		public int TestInt { get; set; }

		[DataMember(Order = 3)]
		public double TestDouble { get; set; }

		[DataMember(Order = 4)]
		public long TestLong { get; set; }

		[DataMember(Order = 5)]
		public short TestShort { get; set; }

		[DataMember(Order = 6)]
		public string TestString { get; set; }

		[DataMember(Order = 7)]
		public DateTime TestDateTime { get; set; }

		[DataMember(Order = 8)]
		public byte TestByte { get; set; }

		[DataMember(Order = 9)]
		public byte[] TestByteArray { get; set; }

		[DataMember(Order = 10)]
		public List<int> TestList { get; set; }

		[DataMember(Order = 11)]
		public sbyte TestsByte { get; set; }

		[DataMember(Order = 12)]
		public uint TestuInt { get; set; }

		[DataMember(Order = 13)]
		public char TestChar { get; set; }

		[DataMember(Order = 14)]
		public decimal TestDecimal { get; set; }

		[DataMember(Order = 15)]
		public List<SubTestData> Children { get; set; }

		[IgnoreDataMember]
		public int DontGo { get; set; }
	}

	[DataContract]
	public class SubTestData
	{
		public SubTestData()
		{
			Name = "default";
		}

		[DataMember(Order = 1)]
		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			return Name == ((SubTestData)obj).Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}

	[DataContract]
	public class ContainerWrapper
	{
		[DataMember(Order = 1, IsRequired = false)]
		public ISerializedContainer Container { get; set; }

		[DataMember(Order = 2, IsRequired = false)]
		public ISerializedContainer AlwaysNull { get; set; }
	}


}