using ImGuiNET;
using System.Reflection;

namespace OpenTPW;
internal class RidesTab : BaseTab
{
	private int selectedRide = 0;

	public override void Draw()
	{
		ImGui.Begin( "Ride VM disassembly view" );

		const int maxColumns = 8;

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

		ImGui.BeginChild( "ride_disassembly" );
		ImGui.Columns( maxColumns, null, false );

		//
		// Disassembly view
		//
		{
			int instructionIndex = 0;
			int labelOffset = 0;

			foreach ( var instruction in vm.Instructions )
			{
				if ( vm.Branches.Any( b => b.CompiledOffset == labelOffset - 1 ) )
				{
					ImGui.Text( $".label_{labelOffset - 1}" );
					ImGui.NextColumn();

					for ( int i = 0; i < maxColumns - 1; ++i )
						ImGui.NextColumn();
				}

				ImGui.Text( vm.CurrentPos == instructionIndex ? ">" : "" );
				ImGui.NextColumn();
				ImGui.Text( instruction.opcode.ToString() );

				if ( ImGui.IsItemHovered() )
				{
					ImGui.BeginTooltip();
					ImGui.Text( instruction.opcode.ToString() );

					var instructionHandler = vm.FindOpcodeHandler( instruction.opcode )?.GetCustomAttribute<OpcodeHandlerAttribute>();

					if ( instructionHandler == null )
						ImGui.Text( "No handler found." );
					else
						ImGui.Text( instructionHandler.Description );

					ImGui.EndTooltip();
				}

				ImGui.NextColumn();
				for ( int i = 0; i < maxColumns - 2; ++i )
				{
					if ( i <= instruction.operands.Length - 1 && instruction.operands.Length > 0 )
						ImGui.Text( instruction.operands[i].ToString() );

					ImGui.NextColumn();
				}

				instructionIndex++;
				labelOffset += instruction.GetCount();
			}
		}

		ImGui.Columns( 1 );
		ImGui.EndChild();

		ImGui.End();
	}
}
