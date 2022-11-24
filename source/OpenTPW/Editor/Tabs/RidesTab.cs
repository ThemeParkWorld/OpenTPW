using ImGuiNET;
using System.Reflection;

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
			if ( ImGui.Button( vm.IsRunning ? "Stop" : "Start" ) )
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

				var opcodeHandlerFunc = vm.FindOpcodeHandler( instruction.opcode );
				var opcodeHandler = opcodeHandlerFunc?.GetCustomAttribute<OpcodeHandlerAttribute>();

				ImGui.SameLine();

				EditorHelpers.DrawColoredText( $"{instruction.opcode}", (opcodeHandler == null) ? OneDark.Warning : OneDark.Instruction );

				// Opcode info tooltip
				if ( ImGui.IsItemHovered() )
				{
					ImGui.BeginTooltip();

					if ( opcodeHandlerFunc != null )
					{
						var parameters = opcodeHandlerFunc.GetParameters()[1..];
						var paramStr = string.Join( ", ", parameters.Select( x => x.Name ) );

						if ( parameters.Length > 0 )
							paramStr = $" {paramStr} ";

						ImGui.Text( $"{instruction.opcode}({paramStr})" );
					}
					else
					{
						ImGui.Text( $"Unimplemented opcode!" );
					}

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
				if ( ImGui.BeginTable( "vm_variables", 2, ImGuiTableFlags.PadOuterX ) )
				{
					ImGui.TableNextColumn();
					ImGui.TableHeader( "Name" );
					ImGui.TableNextColumn();
					ImGui.TableHeader( "Value" );
					ImGui.TableNextRow();

					for ( int i = 0; i < vm.Variables.Count; i++ )
					{
						var variableValue = vm.Variables[i];
						var variableName = vm.VariableNames[i];

						ImGui.TableNextColumn();
						ImGui.SetNextItemWidth( -1 );
						ImGui.LabelText( $"##{variableName}_label", variableName );

						ImGui.TableNextColumn();
						ImGui.SetNextItemWidth( -1 );
						if ( ImGui.InputInt( $"##{variableName}input", ref variableValue ) )
							vm.Variables[i] = variableValue;
					}

					ImGui.EndTable();
				}

				ImGui.Separator();

				EditorHelpers.ApplyPadding();
				EditorHelpers.DrawColoredText( $"Flags:\n{vm.Flags}", OneDark.Generic );

				EditorHelpers.ApplyPadding();
				var stackStr = string.Join( "\n", vm.Stack.Select( x => x.ToString() ) );
				EditorHelpers.DrawColoredText( $"Stack:\n{stackStr}", OneDark.Generic );

				if ( ImGui.BeginTable( "vm_config", 2, ImGuiTableFlags.PadOuterX ) )
				{
					var properties = vm.Config.GetType().GetProperties();

					ImGui.TableNextColumn();
					ImGui.TableHeader( "Name" );
					ImGui.TableNextColumn();
					ImGui.TableHeader( "Value" );
					ImGui.TableNextRow();

					for ( int i = 0; i < properties.Length; i++ )
					{
						int propertyValue = (int)properties[i].GetValue( vm.Config );
						var propertyName = properties[i].Name;

						ImGui.TableNextColumn();
						ImGui.SetNextItemWidth( -1 );
						ImGui.LabelText( $"##{propertyName}_label", propertyName );

						ImGui.TableNextColumn();
						ImGui.SetNextItemWidth( -1 );
						if ( ImGui.InputInt( $"##{propertyName}input", ref propertyValue ) )
							properties[i].SetValue( vm.Config, propertyValue );
					}

					ImGui.EndTable();
				}

				ImGui.EndTabItem();
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
