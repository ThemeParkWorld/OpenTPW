namespace OpenTPW;

static class OneDark
{
	public static System.Numerics.Vector4 Background => MathExtensions.GetColor( "#282C34" );
	public static System.Numerics.Vector4 Variable => MathExtensions.GetColor( "#E06C75" );
	public static System.Numerics.Vector4 String => MathExtensions.GetColor( "#98C379" );
	public static System.Numerics.Vector4 Literal => MathExtensions.GetColor( "#E5C07B" );
	public static System.Numerics.Vector4 Label => MathExtensions.GetColor( "#61AFEF" );
	public static System.Numerics.Vector4 Instruction => MathExtensions.GetColor( "#C678DD" );
	public static System.Numerics.Vector4 Comment => MathExtensions.GetColor( "#56B6C2" );
	public static System.Numerics.Vector4 Generic => MathExtensions.GetColor( "#ABB2BF" );
	public static System.Numerics.Vector4 Step => MathExtensions.GetColor( "#C8CC76" );

	public static System.Numerics.Vector4 Error => MathExtensions.GetColor( "#E06B74" );
	public static System.Numerics.Vector4 Trace => MathExtensions.GetColor( "#ABB2BF" );
	public static System.Numerics.Vector4 Warning => MathExtensions.GetColor( "#E5C07B" );
	public static System.Numerics.Vector4 Info => MathExtensions.GetColor( "#62AEEF" );

	public static System.Numerics.Vector4 DullGeneric => MathExtensions.GetColor( "#4f5259" );
}
