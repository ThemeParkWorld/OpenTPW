namespace OpenTPW;

partial class Event
{
	public class Window
	{
		public class ResizedAttribute : EventAttribute
		{
			public const string Name = "Event.Window.Resized";
			public ResizedAttribute() : base( Name ) { }
		}
	}
}
