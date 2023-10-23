using ImGuiNET;
using OpenTPW.Formats.String;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW;

[EditorMenu( "Debug/TestConsole" )]
internal class QuietConsoleTab : BaseTab
{
	string consoleInput = "";
	List<ConsoleItem> items = new();
	
	public QuietConsoleTab()
	{
		Logger.QuietLog += ( severity, str ) =>
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
			if ( consoleInput == "BFST" )
			{
				string bfstFile = $"{Settings.Default.GamePath}\\data\\Language\\English\\ENTERTAINER_NAMES.str";
				new BFSTReader( bfstFile );
			}

			if ( consoleInput == "MTU" )
			{
				string file = $"{Settings.Default.GamePath}\\data\\Language\\English\\MBToUni.dat";
				new BFMUReader( file ).CharacterArray();
			}

			Log.Info( $"Console input: '{consoleInput}'" );
			consoleInput = "";
		}

		ImGui.End();
	}
}
