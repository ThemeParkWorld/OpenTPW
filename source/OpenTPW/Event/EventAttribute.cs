namespace OpenTPW;

[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public class EventAttribute : Attribute
{
	public string EventName { get; set; }

	public EventAttribute( string eventName )
	{
		EventName = eventName;
	}
}
