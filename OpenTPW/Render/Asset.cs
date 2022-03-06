namespace OpenTPW
{
	public class Asset
	{
		public string Path { get; set; }
		public static List<Asset> All { get; private set; } = new();
	}
}
