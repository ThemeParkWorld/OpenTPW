using ImGuiNET;
using System.Numerics;

namespace OpenTPW;
internal class RidesTab : BaseTab
{
	private int selectedRide = 0;

	struct Colors
	{
		public System.Numerics.Vector4 Background => MathExtensions.GetColor( "#282C34" );
		public System.Numerics.Vector4 Error => MathExtensions.GetColor( "#E06C75" );
		public System.Numerics.Vector4 String => MathExtensions.GetColor( "#98C379" );
		public System.Numerics.Vector4 Literal => MathExtensions.GetColor( "#E5C07B" );
		public System.Numerics.Vector4 Label => MathExtensions.GetColor( "#61AFEF" );
		public System.Numerics.Vector4 Instruction => MathExtensions.GetColor( "#C678DD" );
		public System.Numerics.Vector4 Comment => MathExtensions.GetColor( "#56B6C2" );
		public System.Numerics.Vector4 Generic => MathExtensions.GetColor( "#ABB2BF" );
	}

	private static Colors colors = new();

	private string Align( string str ) => str.PadRight( 16, ' ' );

	private void DrawColoredText( string str, System.Numerics.Vector4 col, bool align = true )
	{
		ImGui.PushStyleColor( ImGuiCol.Text, col );

		if ( align )
			str = Align( str );
		ImGui.Text( str );

		ImGui.PopStyleColor();
	}

	public override void Draw()
	{
		ImGui.Begin( "Ride VM disassembly view" );

		var rides = Entity.All.OfType<Ride>().ToList();

		//
		// Ride list
		//
		{
			var rideList = Entity.All.OfType<Ride>().ToList().Select( x => x.Name );
			ImGui.Combo( "Ride", ref selectedRide, rideList.ToArray(), rideList.Count() );
		}

		var vm = rides[selectedRide].VM;

		//
		// Toolbar
		//
		{
			if ( ImGui.Button( "Debug" ) )
				vm.Run();

			ImGui.SameLine();
			if ( ImGui.Button( "Step" ) )
				vm.Step();

			ImGui.SameLine();
			ImGui.Text( $"Offset {vm.CurrentPos}" );
		}

		ImGui.Separator();


		//
		// Disassembly view
		//
		ImGui.PushStyleColor( ImGuiCol.ChildBg, colors.Background );
		ImGui.BeginChild( "ride_disassembly" );
		ImGui.PushFont( Editor.MonospaceFont );

		{
			int labelOffset = -1;
			var padding = new System.Numerics.Vector2( 4, 4 );

			foreach ( var instruction in vm.Instructions )
			{
				//
				// Check if we have any branches pointing here
				//
				if ( vm.Branches.Any( b => b.CompiledOffset == labelOffset ) )
				{
					ImGui.NewLine();
					DrawColoredText( $".label_{labelOffset}", colors.Label );
				}

				ImGui.SetCursorPos( ImGui.GetCursorPos() + padding );
				DrawColoredText( $"0x{instruction.offset:X4}: ", colors.Comment );
				ImGui.SameLine();
				DrawColoredText( $"{instruction.opcode}", colors.Instruction );

				//
				// Draw operands
				//
				foreach ( var operand in instruction.operands )
				{
					ImGui.SameLine();

					var color = operand.type switch
					{
						Operand.Type.Variable => colors.Generic,
						Operand.Type.Literal => colors.Literal,
						Operand.Type.String => colors.String,
						Operand.Type.Location => colors.Label,
						_ => colors.Generic
					};

					DrawColoredText( $"{operand}", color );
				}

				labelOffset += instruction.GetCount();
			}
		}

		ImGui.EndChild();
		ImGui.PopStyleColor();
		ImGui.PopFont();

		ImGui.End();
	}
}
