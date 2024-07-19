namespace OpenTPW;

partial class Event
{
	public class FileSystem
	{
		public class HotLoadAttribute : EventAttribute
		{
			public const string Name = "Event.FileSystem.HotLoad";
			public HotLoadAttribute() : base( Name ) { }
		}
	}
}
