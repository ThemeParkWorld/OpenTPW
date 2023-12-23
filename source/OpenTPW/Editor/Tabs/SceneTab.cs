using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace OpenTPW;

[EditorMenu( "Scene/Outliner" )]
internal class SceneTab : BaseTab
{
	private Entity? selectedEntity;

	class ReflectedThing
	{
		public string Name { get; set; }
		public object? Value { get; set; }

		public ReflectedThing( object o, string name )
		{
			Name = name;
			Value = o;
		}

		public ReflectedThing( object o, FieldInfo fieldInfo )
		{
			Name = fieldInfo.Name;
			Value = fieldInfo.GetValue( o );
		}

		public ReflectedThing( object o, PropertyInfo propertyInfo )
		{
			Name = propertyInfo.Name;
			Value = propertyInfo.GetValue( o );
		}
	}

	private void DrawElement( ReflectedThing thing )
	{
		if ( thing.Value is Vector3 vec3 )
		{
			var sysVec3 = vec3.GetSystemVector3();
			ImGui.SetNextItemWidth( -1 );
			ImGui.InputFloat3( $"##thing_{thing.GetHashCode()}", ref sysVec3 );
		}
		else if ( thing.Value is Matrix4x4 mat4 )
		{
			ImGui.SetNextItemWidth( -1 );
			ImGui.Text( $"{mat4.Column1():0.00}" );
			ImGui.SetNextItemWidth( -1 );
			ImGui.Text( $"{mat4.Column2():0.00}" );
			ImGui.SetNextItemWidth( -1 );
			ImGui.Text( $"{mat4.Column3():0.00}" );
			ImGui.SetNextItemWidth( -1 );
			ImGui.Text( $"{mat4.Column4():0.00}" );
		}
		else if ( thing.Value is string str )
		{
			ImGui.SetNextItemWidth( -1 );
			ImGui.InputText( $"##thing_{thing.GetHashCode()}", ref str, 256 );
		}
		else if ( thing.Value is float f )
		{
			ImGui.SetNextItemWidth( -1 );
			ImGui.SliderFloat( $"##thing_{thing.GetHashCode()}", ref f, 0.0f, 1.0f );
		}
		else if ( thing.Value is System.Collections.IList list )
		{
			EditorHelpers.DrawColoredText( $"List: [", OneDark.Info );

			foreach ( var item in list )
			{
				DrawElement( new ReflectedThing( item, $"{item}" ) );
			}

			EditorHelpers.DrawColoredText( $"]", OneDark.Info );
		}
		else
		{
			ImGui.Text( $"{thing.Value}" );
			ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
			ImGui.Text( $"[{thing.Value.GetType()}]" );
			ImGui.PopStyleColor();
		}
	}

	public override void Draw()
	{
		ImGui.Begin( "Scene", ref visible );

		//
		// Hierarchy
		//
		{
			ImGui.SetNextItemWidth( -1 );
			ImGui.BeginListBox( "##hierarchy" );

			foreach ( var entity in Entity.All )
			{
				var startPos = ImGui.GetCursorPos();

				if ( ImGui.Selectable( entity.Name ) )
				{
					selectedEntity = entity;
				}

				ImGui.Separator();
			}

			ImGui.EndListBox();
		}

		ImGui.Separator();

		//
		// Inspector
		//
		{
			if ( selectedEntity != null )
			{
				var selectedEntityType = selectedEntity.GetType();

				foreach ( var group in selectedEntityType.GetMembers().GroupBy( x => x.DeclaringType ) )
				{
					if ( group.Key == typeof( object ) )
						continue;

					ImGui.Text( $"{group.Key}:" );

					if ( ImGui.BeginTable( $"##table_{group}", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.PadOuterX | ImGuiTableFlags.SizingStretchProp ) )
					{
						ImGui.TableSetupColumn( "Name", ImGuiTableColumnFlags.WidthFixed, 100f );
						ImGui.TableSetupColumn( "Value", ImGuiTableColumnFlags.WidthStretch, 1f );

						ImGui.TableNextColumn();
						ImGui.TableHeader( "Name" );
						ImGui.TableNextColumn();
						ImGui.TableHeader( "Value" );

						foreach ( var item in group )
						{
							if ( item.MemberType != MemberTypes.Field && item.MemberType != MemberTypes.Property )
								continue;

							ImGui.TableNextRow();
							ImGui.TableNextColumn();
							ImGui.Text( $"{item.Name}" );
							ImGui.TableNextColumn();

							if ( item.MemberType == MemberTypes.Field )
							{
								var field = item as FieldInfo;
								var thing = new ReflectedThing( selectedEntity, field );

								DrawElement( thing );
							}
							else if ( item.MemberType == MemberTypes.Property )
							{
								var property = item as PropertyInfo;
								var thing = new ReflectedThing( selectedEntity, property );

								DrawElement( thing );
							}
						}

						ImGui.EndTable();
					}
				}
			}
		}

		ImGui.End();
	}
}
