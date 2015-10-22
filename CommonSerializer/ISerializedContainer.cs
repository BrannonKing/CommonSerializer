namespace CommonSerializer
{
	public interface ISerializedContainer
	{
		int Count { get; }
		bool CanRead { get; }
		bool CanWrite { get; }
	}
}
