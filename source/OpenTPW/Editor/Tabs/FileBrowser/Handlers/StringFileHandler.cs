using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW;
[FileHandler( @"\.str", "content/icons/strings.png" )]
public class StringFileHandler : BaseFileHandler
{
	private BFSTReader reader;

	public StringFileHandler( byte[] fileData ) : base( fileData )
	{
		using var stream = new BFSTStream( fileData );
		reader = new BFSTReader( stream );
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"BFST File Contents:" );
		ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		var bfstOutput = reader.ReadFile();

		foreach( var output in  bfstOutput )
		{
			ImGui.Text( output );
		}

		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}

}
