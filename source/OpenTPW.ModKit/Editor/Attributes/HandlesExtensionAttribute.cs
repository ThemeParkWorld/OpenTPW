namespace OpenTPW.ModKit;

[System.AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
internal sealed class HandlesExtensionAttribute : Attribute
{
	public readonly string Extension;

	public HandlesExtensionAttribute( string extension )
	{
		Extension = extension;
	}
}
