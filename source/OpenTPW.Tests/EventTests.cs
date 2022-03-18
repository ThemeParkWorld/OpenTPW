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

			public class TestEventWithParamsAttribute : EventAttribute
			{
				public const string Name = "Event.Test.TestEventWithParams";
				public TestEventWithParamsAttribute() : base( Name ) { }
			}
		}
	}

	static bool staticEventHasRun = false;
	static bool staticEventWithParamsHasRun = false;

	static bool instantiatedEventHasRun = false;
	static bool instantiatedEventWithParamsHasRun = false;

	[TestMethod]
	public void RegisterAndRunStaticEvent()
	{
		Event.Register( this );
		Event.Run( "Event.Test.TestEvent" );

		Assert.IsTrue( staticEventHasRun );
	}

	[TestMethod]
	public void RegisterAndRunStaticEventWithParams()
	{
		Event.Register( this );
		Event.Run( "Event.Test.TestEventWithParams", "Test" );

		Assert.IsTrue( staticEventWithParamsHasRun );
	}

	[TestMethod]
	public void RegisterAndRunInstantiatedEvent()
	{
		Event.Register( this );
		Event.Run( "Event.Test.TestEvent" );

		Assert.IsTrue( instantiatedEventHasRun );
	}

	[TestMethod]
	public void RegisterAndRunInstantiatedEventWithParams()
	{
		Event.Register( this );
		Event.Run( "Event.Test.TestEventWithParams", "Test" );

		Assert.IsTrue( instantiatedEventWithParamsHasRun );
	}

	[TestEvent.Test.TestEvent]
	public void InstantiatedEvent()
	{
		instantiatedEventHasRun = true;
	}

	[TestEvent.Test.TestEventWithParams]
	public static void InstantiatedEventWithParams( string parameter )
	{
		instantiatedEventWithParamsHasRun = parameter == "Test";
	}

	[TestEvent.Test.TestEventWithParams]
	public static void StaticEventWithParams( string parameter )
	{
		staticEventWithParamsHasRun = parameter == "Test";
	}

	[TestEvent.Test.TestEvent]
	public static void StaticEvent()
	{
		staticEventHasRun = true;
	}
}
