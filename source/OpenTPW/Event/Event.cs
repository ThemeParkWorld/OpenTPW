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
