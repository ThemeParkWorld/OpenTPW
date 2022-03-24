namespace OpenTPW;

[System.AttributeUsage( AttributeTargets.Method, Inherited = false, AllowMultiple = false )]
sealed class FileManagerAttribute : Attribute
{
	public string RegexPattern { get; set; }
	public bool IsDefault { get; set; }

	public FileManagerAttribute( string extension, bool isDefault = false )
	{
		RegexPattern = extension;
		IsDefault = isDefault;
	}
}
