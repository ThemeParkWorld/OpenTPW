namespace OpenTPW;

partial class Event
{
	public class Game
	{
		public class LoadAttribute : EventAttribute
		{
			const string EventName = "Event.Game.Load";
			public LoadAttribute() : base( EventName ) { }
		}
	}
}
