namespace OpenTPW;

public partial class RideVM
{
	[Flags]
	public enum VMFlags
	{
		None,

		Sign = 1,
		Zero = 2,
		Crit = 4,

		All = Sign | Zero | Crit
	}
}
