namespace OpenTPW;

partial class Event
{
	public class Game
	{
		public class LoadAttribute : EventAttribute
		{
			public const string Name = "Event.Game.Load";
			public LoadAttribute() : base( Name ) { }
		}
	}
}
