# CommonSerializer

This library collection provides a common interface for common .NET serialization engines. It allows for quickly switching between the various serializers for comparing performance, features, etc.

These serializers are supported:
1. Newtonsoft Json.NET
2. MsgPack.CLI
3. Protobuf-net
4. Jil
5. PowerJSON (built from fastJSON)

If there are others that you want supported, please file an issue requesting that.

The first three serializers listed support a feature for partial serialization. Consider that you might want to deserialize an outer object first so that you can determine the right data type for some inner object. The recommended method for accomplishing this works like this:

```csharp
[DataContract]
public class RemoteProcedureCall
{
   [DataMember(Order = 1)]
   public string MethodName {get;set;}
}

[DataContract]
public class RemoteProcedureCall<T>: RemoteProcedureCall
{
   [DataMember(Order = 1)]
   public T Arguments {get;set;}
}
```

The interface supports a method to register inheritors: `RegisterSubtype`. That will be necessary for serializers like Protobuf-net (or you can reference the Protobuf-net assembly directly and use its attributes).

However, you can use partial serialization like this:

```csharp
[DataContract]
public class RemoteProcedureCall
{
   [DataMember(Order = 1)]
   public string MethodName {get;set;}
   
   [DataMember(Order = 2)]
   public ISerializedContainer Arguments {get;set;}
}

...

var serializer = new ProtobufCommonSerializer();
var msg = new RemoteProcedureCall { MethodName = "method" };
msg.Arguments = serializer.GenerateContainer();
serializer.Serialize(msg.Arguments, myFirstArgument);
serializer.Serialize(msg.Arguments, mySecondArgument);
serializer.Serialize(targetStream, msg);

// and similarly for the reverse
var msg2 = serializer.Deserialize<RemoteProcedureCall>(targetStream);
var argument1 = serializer.Deserialize<MyFirstArgumentType>(msg2.Arguments);
```