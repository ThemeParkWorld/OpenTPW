namespace OpenTPW;

[System.AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
sealed class FileHandlerAttribute : Attribute
{
	public string RegexPattern { get; set; }
	public string Icon { get; set; }
	public bool IsDefault { get; set; }

	public FileHandlerAttribute( string extension, string icon = "content/icons/document.png", bool isDefault = false )
	{
		RegexPattern = extension;
		Icon = icon;
		IsDefault = isDefault;
	}
}
