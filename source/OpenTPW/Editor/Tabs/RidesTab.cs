using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace OpenTPW;
internal class RidesTab : BaseTab
{
	private int selectedRide = 0;

	struct Colors
	{
		public System.Numerics.Vector4 Background => MathExtensions.GetColor( "#282C34" );
		public System.Numerics.Vector4 Variable => MathExtensions.GetColor( "#E06C75" );
		public System.Numerics.Vector4 String => MathExtensions.GetColor( "#98C379" );
		public System.Numerics.Vector4 Literal => MathExtensions.GetColor( "#E5C07B" );
		public System.Numerics.Vector4 Label => MathExtensions.GetColor( "#61AFEF" );
		public System.Numerics.Vector4 Instruction => MathExtensions.GetColor( "#C678DD" );
		public System.Numerics.Vector4 Comment => MathExtensions.GetColor( "#56B6C2" );
		public System.Numerics.Vector4 Generic => MathExtensions.GetColor( "#ABB2BF" );
		public System.Numerics.Vector4 Step => MathExtensions.GetColor( "#C8CC76" );
		public System.Numerics.Vector4 Black => MathExtensions.GetColor( "#000000" );
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

	private Dictionary<int, float> labelPositions = new(); // TODO: Refresh this on script change

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

			for ( int i = 0; i < vm.Instructions.Count; i++ )
			{
				Instruction instruction = vm.Instructions[i];

				//
				// Check if we have any branches pointing here
				//
				if ( vm.Branches.Any( b => b.CompiledOffset == labelOffset ) )
				{
					ImGui.NewLine();

					DrawColoredText( $".label_{labelOffset}", colors.Label );

					if ( !labelPositions.ContainsKey( labelOffset ) )
						labelPositions.Add( labelOffset, ImGui.GetCursorPosY() );
				}

				ImGui.SetCursorPos( ImGui.GetCursorPos() + padding );

				// Draw file offset
				{
					var col = (i == vm.CurrentPos) ? colors.Step : colors.Generic;
					DrawColoredText( $"0x{instruction.offset:X4}: ", col );
				}

				ImGui.SameLine();
				DrawColoredText( $"{instruction.opcode}", colors.Instruction );

				// Opcode info tooltip
				if ( ImGui.IsItemHovered() )
				{
					ImGui.BeginTooltip();
					ImGui.Text( $"{instruction.opcode}" );

					var opcodeHandler = vm.FindOpcodeHandler( instruction.opcode )?.GetCustomAttribute<OpcodeHandlerAttribute>();

					if ( opcodeHandler != null )
					{
						ImGui.PushFont( Editor.SansSerifFont );
						ImGui.Text( $"{opcodeHandler.Description}" );
						ImGui.PopFont();
					}
					ImGui.End();
				}

				//
				// Draw operands
				//
				foreach ( var operand in instruction.operands )
				{
					ImGui.SameLine();

					var color = operand.type switch
					{
						Operand.Type.Variable => colors.Variable,
						Operand.Type.Literal => colors.Literal,
						Operand.Type.String => colors.String,
						Operand.Type.Location => colors.Label,
						_ => colors.Generic
					};

					DrawColoredText( $"{operand}", color );

					// Link hover, jump to on click
					if ( ImGui.IsItemHovered() && operand.type == Operand.Type.Location )
					{
						var itemP1 = ImGui.GetItemRectMin();
						var itemP2 = ImGui.GetItemRectMax();

						itemP1.Y = itemP2.Y;
						itemP2.X = itemP1.X + ImGui.CalcTextSize( operand.ToString() ).X;

						var drawList = ImGui.GetWindowDrawList();
						var underlineCol = ImGui.GetColorU32( color );
						drawList.AddLine( itemP1, itemP2, underlineCol );

						if ( ImGui.IsItemClicked() )
						{
							// Jump to
							if ( labelPositions.TryGetValue( operand.Value, out var position ) )
							{
								Log.Trace( "Jumped to " + operand.Value );
								ImGui.SetScrollY( position - 32f );
							}
							else
							{
								Log.Trace( "Couldn't jump to " + operand.Value );
							}
						}
					}
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
