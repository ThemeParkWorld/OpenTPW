using ImGuiNET;

namespace OpenTPW;

// Originally by haldean:
// https://github.com/ocornut/imgui/issues/2265#issuecomment-465432091

[EditorMenu( "ImGUI/Themer" )]
internal class ThemerTab : BaseTab
{
	System.Numerics.Vector4 baseCol = new( 0.502f, 0.075f, 0.256f, 1.0f );
	System.Numerics.Vector4 bgCol = new( 0.200f, 0.220f, 0.270f, 1.0f );
	System.Numerics.Vector4 textCol = new( 0.860f, 0.930f, 0.890f, 1.0f );

	float high_val = 0.8f;
	float mid_val = 0.5f;
	float low_val = 0.3f;
	float window_offset = -0.2f;

	public override void Draw()
	{
		System.Numerics.Vector4 CalcHigh( float alpha )
		{
			System.Numerics.Vector4 res = new( 0, 0, 0, alpha );
			ImGui.ColorConvertRGBtoHSV( baseCol.X, baseCol.Y, baseCol.Z, out res.X, out res.Y, out res.Z );
			res.Z = high_val;
			ImGui.ColorConvertHSVtoRGB( res.X, res.Y, res.Z, out res.X, out res.Y, out res.Z );
			return res;
		}

		System.Numerics.Vector4 CalcMid( float alpha )
		{
			System.Numerics.Vector4 res = new( 0, 0, 0, alpha );
			ImGui.ColorConvertRGBtoHSV( baseCol.X, baseCol.Y, baseCol.Z, out res.X, out res.Y, out res.Z );
			res.Z = mid_val;
			ImGui.ColorConvertHSVtoRGB( res.X, res.Y, res.Z, out res.X, out res.Y, out res.Z );
			return res;
		}

		System.Numerics.Vector4 CalcLow( float alpha )
		{
			System.Numerics.Vector4 res = new( 0, 0, 0, alpha );
			ImGui.ColorConvertRGBtoHSV( baseCol.X, baseCol.Y, baseCol.Z, out res.X, out res.Y, out res.Z );
			res.Z = low_val;
			ImGui.ColorConvertHSVtoRGB( res.X, res.Y, res.Z, out res.X, out res.Y, out res.Z );
			return res;
		}

		System.Numerics.Vector4 CalcBg( float alpha, float offset = 0.0f )
		{
			System.Numerics.Vector4 res = new( 0, 0, 0, alpha );
			ImGui.ColorConvertRGBtoHSV( bgCol.X, bgCol.Y, bgCol.Z, out res.X, out res.Y, out res.Z );
			res.Z += offset;
			ImGui.ColorConvertHSVtoRGB( res.X, res.Y, res.Z, out res.X, out res.Y, out res.Z );
			return res;
		}

		System.Numerics.Vector4 CalcText( float alpha )
		{
			return new( textCol.X, textCol.Y, textCol.Z, alpha );
		}

		ImGui.Begin( "Themer" );
		ImGui.ColorEdit4( "baseCol", ref baseCol, ImGuiColorEditFlags.PickerHueWheel );
		ImGui.ColorEdit4( "bg", ref bgCol, ImGuiColorEditFlags.PickerHueWheel );
		ImGui.ColorEdit4( "text", ref textCol, ImGuiColorEditFlags.PickerHueWheel );
		ImGui.SliderFloat( "high", ref high_val, 0, 1 );
		ImGui.SliderFloat( "mid", ref mid_val, 0, 1 );
		ImGui.SliderFloat( "low", ref low_val, 0, 1 );
		ImGui.SliderFloat( "window", ref window_offset, -0.4f, 0.4f );

		var style = ImGui.GetStyle();

		style.Colors[(int)ImGuiCol.Text] = CalcText( 0.78f );
		style.Colors[(int)ImGuiCol.TextDisabled] = CalcText( 0.28f );
		style.Colors[(int)ImGuiCol.WindowBg] = CalcBg( 1.00f, window_offset );
		style.Colors[(int)ImGuiCol.ChildBg] = CalcBg( 0.58f );
		style.Colors[(int)ImGuiCol.PopupBg] = CalcBg( 0.9f );
		style.Colors[(int)ImGuiCol.Border] = CalcBg( 0.6f, -0.05f );
		style.Colors[(int)ImGuiCol.BorderShadow] = CalcBg( 0.0f, 0.0f );
		style.Colors[(int)ImGuiCol.FrameBg] = CalcBg( 1.00f );
		style.Colors[(int)ImGuiCol.FrameBgHovered] = CalcMid( 0.78f );
		style.Colors[(int)ImGuiCol.FrameBgActive] = CalcMid( 1.00f );
		style.Colors[(int)ImGuiCol.TitleBg] = CalcLow( 1.00f );
		style.Colors[(int)ImGuiCol.TitleBgActive] = CalcHigh( 1.00f );
		style.Colors[(int)ImGuiCol.TitleBgCollapsed] = CalcBg( 0.75f );
		style.Colors[(int)ImGuiCol.MenuBarBg] = CalcBg( 0.47f );
		style.Colors[(int)ImGuiCol.ScrollbarBg] = CalcBg( 1.00f );
		style.Colors[(int)ImGuiCol.ScrollbarGrab] = CalcLow( 1.00f );
		style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = CalcMid( 0.78f );
		style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = CalcMid( 1.00f );
		style.Colors[(int)ImGuiCol.CheckMark] = CalcHigh( 1.00f );
		style.Colors[(int)ImGuiCol.SliderGrab] = CalcBg( 1.0f, .1f );
		style.Colors[(int)ImGuiCol.SliderGrabActive] = CalcHigh( 1.0f );
		style.Colors[(int)ImGuiCol.Button] = CalcBg( 1.0f, .2f );
		style.Colors[(int)ImGuiCol.ButtonHovered] = CalcMid( 1.00f );
		style.Colors[(int)ImGuiCol.ButtonActive] = CalcHigh( 1.00f );
		style.Colors[(int)ImGuiCol.Header] = CalcMid( 0.76f );
		style.Colors[(int)ImGuiCol.HeaderHovered] = CalcMid( 0.86f );
		style.Colors[(int)ImGuiCol.HeaderActive] = CalcHigh( 1.00f );
		style.Colors[(int)ImGuiCol.ResizeGrip] = new( 0.47f, 0.77f, 0.83f, 0.04f );
		style.Colors[(int)ImGuiCol.ResizeGripHovered] = CalcMid( 0.78f );
		style.Colors[(int)ImGuiCol.ResizeGripActive] = CalcMid( 1.00f );
		style.Colors[(int)ImGuiCol.PlotLines] = CalcText( 0.63f );
		style.Colors[(int)ImGuiCol.PlotLinesHovered] = CalcMid( 1.00f );
		style.Colors[(int)ImGuiCol.PlotHistogram] = CalcText( 0.63f );
		style.Colors[(int)ImGuiCol.PlotHistogramHovered] = CalcMid( 1.00f );
		style.Colors[(int)ImGuiCol.TextSelectedBg] = CalcMid( 0.43f );
		style.Colors[(int)ImGuiCol.ModalWindowDimBg] = CalcBg( 0.73f );
		style.Colors[(int)ImGuiCol.Tab] = CalcBg( 0.40f );
		style.Colors[(int)ImGuiCol.TabHovered] = CalcHigh( 1.00f );
		style.Colors[(int)ImGuiCol.TabActive] = CalcMid( 1.00f );
		style.Colors[(int)ImGuiCol.TabUnfocused] = CalcBg( 0.40f );
		style.Colors[(int)ImGuiCol.TabUnfocusedActive] = CalcBg( 0.70f );
		style.Colors[(int)ImGuiCol.DockingPreview] = CalcHigh( 0.30f );

		if ( ImGui.Button( "Export" ) )
		{
			ImGui.LogToTTY();
			ImGui.LogText( "var colors = ImGui.GetStyle().Colors;\n" );

			for ( int i = 0; i < (int)ImGuiCol.COUNT; i++ )
			{
				var col = style.Colors[i];
				var name = ImGui.GetStyleColorName( (ImGuiCol)i );
				ImGui.LogText( $"colors[(int)ImGuiCol.{name}] = new({col.X:0.00}f, {col.Y:0.00}f, {col.Z:0.00}f, {col.W:0.00}f);\n" );
			}

			ImGui.LogFinish();
		}

		ImGui.End();
	}
}
