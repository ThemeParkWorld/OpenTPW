using ImGuiNET;
using System.Numerics;

namespace OpenTPW;

[EditorMenu( "Debug/Console" )]
internal class ConsoleTab : BaseTab
{
	List<ConsoleItem> items = new();
	string consoleInput = "";

	struct ConsoleItem
	{
		public Vector4 Color { get; set; }
		public string Text { get; set; }

		public ConsoleItem( Vector4 color, string text )
		{
			Color = color;
			Text = text;
		}
	}

	public ConsoleTab()
	{
		Logger.OnLog += ( severity, str ) =>
		{
			var color = severity switch
			{
				Logger.Level.Trace => OneDark.Trace,
				Logger.Level.Info => OneDark.Info,
				Logger.Level.Warning => OneDark.Warning,
				Logger.Level.Error => OneDark.Error,
				_ => OneDark.Info,
			};

			items.Add( new ConsoleItem( color, str ) );
		};
	}

	public override void Draw()
	{
		ImGui.Begin( "Console", ref visible );

		ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
		ImGui.BeginChild( "logs", new System.Numerics.Vector2( 0, -32 ) );
		for ( int i = 0; i < items.Count; i++ )
		{
			var line = items[i];

			ImGui.PushStyleColor( ImGuiCol.Text, line.Color );
			ImGui.SetCursorPosX( 5 );
			ImGui.TextWrapped( line.Text );
			ImGui.PopStyleColor();
		}

		if ( ImGui.GetScrollY() >= ImGui.GetScrollMaxY() )
			ImGui.SetScrollHereY( 1.0f );

		ImGui.EndChild();
		ImGui.PopStyleColor();

		ImGui.SetNextItemWidth( -58 );
		ImGui.InputText( "##console_input", ref consoleInput, 512 );
		ImGui.SameLine();

		if ( ImGui.Button( "Submit" ) )
		{
			Log.Info( $"Console input: '{consoleInput}'" );
			consoleInput = "";
		}

		ImGui.End();
	}
}
