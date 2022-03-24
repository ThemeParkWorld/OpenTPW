namespace OpenTPW;

[System.AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
sealed class FileHandlerAttribute : Attribute
{
	public string RegexPattern { get; set; }
	public bool IsDefault { get; set; }

	public FileHandlerAttribute( string extension, bool isDefault = false )
	{
		RegexPattern = extension;
		IsDefault = isDefault;
	}
}
