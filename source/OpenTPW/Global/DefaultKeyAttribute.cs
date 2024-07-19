[System.AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
sealed class DefaultKeyAttribute : Attribute
{
	public Veldrid.Key[] Keys;

	public DefaultKeyAttribute( params Veldrid.Key[] keys )
	{
		Keys = keys;
	}
}
