using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenTPW.Tests;

[TestClass]
public class EventTests
{
	partial class TestEvent
	{
		public class Test
		{
			public class TestEventAttribute : EventAttribute
			{
				public const string Name = "Event.Test.TestEvent";
				public TestEventAttribute() : base( Name ) { }
			}
		}
	}

	static bool eventHasRun = false;

	[TestMethod]
	public void RegisterAndRunStaticEvent()
	{
		Event.Register( this );
		Event.Run( "Event.Test.TestEvent" );

		Assert.IsTrue( eventHasRun );
	}

	[TestEvent.Test.TestEvent]
	public static void StaticEvent()
	{
		eventHasRun = true;
	}
}
