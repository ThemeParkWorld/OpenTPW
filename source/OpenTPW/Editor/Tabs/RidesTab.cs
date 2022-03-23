using ImGuiNET;
using System.Reflection;
using System.Security.Permissions;

namespace OpenTPW;

[EditorMenu( "Ride/Disassembly" )]
internal class RidesTab : BaseTab
{
	private int selectedRide = 0;

	private Dictionary<int, float> labelPositions = new(); // TODO: Refresh this on script change

	public override void Draw()
	{
		ImGui.Begin( "Ride VM disassembly view", ref visible );

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

		ImGui.Columns( 2 );

		//
		// Disassembly view
		//
		ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
		ImGui.BeginChild( "ride_disassembly" );
		ImGui.PushFont( Editor.MonospaceFont );

		{
			int labelOffset = -1;

			EditorHelpers.ApplyPadding();
			EditorHelpers.DrawColoredText( $"; ************* Initialise ***************", OneDark.Comment );

			for ( int i = 0; i < vm.Instructions.Count; i++ )
			{
				Instruction instruction = vm.Instructions[i];

				//
				// Check if we have any branches pointing here
				//
				if ( vm.Branches.Any( b => b.CompiledOffset == labelOffset ) )
				{
					ImGui.NewLine();
					EditorHelpers.ApplyPadding();

					EditorHelpers.DrawColoredText( $".label_{labelOffset}:", OneDark.Label );

					if ( !labelPositions.ContainsKey( labelOffset ) )
						labelPositions.Add( labelOffset, ImGui.GetCursorPosY() );
				}

				EditorHelpers.ApplyPadding();

				// Draw file offset
				{
					var col = (i == vm.CurrentPos) ? OneDark.Step : OneDark.Generic;
					EditorHelpers.DrawColoredText( $"0x{instruction.offset:X4}: ", col );
				}

				ImGui.SameLine();
				EditorHelpers.DrawColoredText( $"{instruction.opcode}", OneDark.Instruction );

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

					ImGui.EndTooltip();
				}

				//
				// Draw operands
				//
				foreach ( var operand in instruction.operands )
				{
					ImGui.SameLine();

					var color = operand.type switch
					{
						Operand.Type.Variable => OneDark.Variable,
						Operand.Type.Literal => OneDark.Literal,
						Operand.Type.String => OneDark.String,
						Operand.Type.Location => OneDark.Label,
						_ => OneDark.Generic
					};

					EditorHelpers.DrawColoredText( $"{operand}", color );

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

						ImGui.BeginTooltip();
						ImGui.PushFont( Editor.SansSerifFont );
						ImGui.Text( $"Click to go to label_{operand.Value}" );
						ImGui.PopFont();
						ImGui.EndTooltip();

						if ( ImGui.IsItemClicked() )
						{
							// Jump to
							if ( labelPositions.TryGetValue( operand.Value, out var position ) )
							{
								Log.Trace( "Jumped to " + operand.Value );
								ImGui.SetScrollY( position + 32.0f );
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

		//
		// Memory view
		//
		ImGui.NextColumn();
		ImGui.BeginChild( "ride_memory" );

		{
			ImGui.BeginTabBar( "ride_memory_tabs" );

			if ( ImGui.BeginTabItem( "Variables" ) )
			{
				ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
				ImGui.BeginChild( "ride_variables" );
				ImGui.PushFont( Editor.MonospaceFont );

				for ( int i = 0; i < vm.Variables.Count; i++ )
				{
					var variableValue = vm.Variables[i];
					var variableName = vm.VariableNames[i];
					EditorHelpers.DrawColoredText( $"{variableName}", OneDark.Generic );
					ImGui.SameLine();
					EditorHelpers.DrawColoredText( $"{variableValue}", OneDark.Generic );
				}

				ImGui.PopFont();
				ImGui.EndChild();
				ImGui.EndTabItem();
				ImGui.PopStyleColor();
			}

			if ( ImGui.BeginTabItem( "Hex" ) )
			{
				ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
				ImGui.BeginChild( "ride_hex" );
				ImGui.PushFont( Editor.MonospaceFont );

				for ( int i = 0; i < vm.FileData.Length; ++i )
				{
					if ( i % 16 == 0 )
					{
						EditorHelpers.DrawColoredText( $"0x{i:X4}:", OneDark.Generic );
					}

					ImGui.SameLine();

					var b = vm.FileData[i];
					var color = OneDark.Generic;
					if ( b == 0 )
						color = OneDark.DullGeneric;

					EditorHelpers.DrawColoredText( $"{b:X2}", color, align: false );
				}

				ImGui.PopFont();
				ImGui.EndChild();
				ImGui.EndTabItem();
				ImGui.PopStyleColor();
			}
		}

		ImGui.EndChild();

		ImGui.Columns( 1 );
		ImGui.End();
	}
}
