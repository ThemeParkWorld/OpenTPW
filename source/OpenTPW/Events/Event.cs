using System.Linq;
using System.Reflection;

namespace OpenTPW;

public static partial class Event
{
	struct EventRef
	{
		public EventRef( string name, MethodInfo method )
		{
			this.name = name;
			this.method = method;
		}

		public string name { get; set; }
		public MethodInfo method { get; set; }


	}
	private static List<EventRef> events = new();

	public static void Register( object obj )
	{
		var attributes = obj.GetType().GetMethods()
			.Where( m => m.GetCustomAttribute<EventAttribute>() != null )
			.Select( m => new EventRef( m.GetCustomAttribute<EventAttribute>().EventName, m ) );

		events.AddRange( attributes );
	}

	public static void Run( string name )
	{
		events.ForEach( e =>
		{
			if ( e.name == name )
				e.method?.Invoke( null, null );
		} );
	}
}

[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public class EventAttribute : Attribute
{
	public string EventName { get; set; }

	public EventAttribute( string eventName )
	{
		EventName = eventName;
	}
}

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
