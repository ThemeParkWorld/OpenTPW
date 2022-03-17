using System.Reflection;

namespace OpenTPW;

public class RideVM
{
	public struct VMConfig
	{
		public int StackSize { get; set; }
		public int LimboSize { get; set; }
		public int BounceSize { get; set; }
		public int WalkSize { get; set; }
		public int TimeSlice { get; set; }
	}
	public struct VMFlags
	{
		public bool Sign { get; set; }
		public bool Zero { get; set; }
		public bool Crit { get; set; }
	}

	private readonly RsseqFile rsseqFile;

	public string ScriptName { get; set; } = "Unnamed";

	public bool Run { get; set; }
	public int CurrentPos { get; set; }
	public string Disassembly => rsseqFile.Disassembly;
	public List<Instruction> Instructions { get; } = new List<Instruction>();

	/// <summary>
	/// Key: String offset
	/// Value: String value
	/// </summary>
	public Dictionary<long, string> Strings { get; } = new Dictionary<long, string>();
	public List<int> Variables { get; set; } = new List<int>();
	public List<string> VariableNames { get; set; } = new List<string>();
	public VMFlags Flags { get; private set; } = new VMFlags();
	public VMConfig Config { get; set; } = new VMConfig();
	public List<Branch> Branches { get; set; } = new List<Branch>();

	public RideVM( Stream stream )
	{
		rsseqFile = new RsseqFile( this );
		rsseqFile.ReadFile( stream );
	}

	public void Step()
	{
		var instruction = Instructions[CurrentPos++];
		Log.Trace( $"Invoking {instruction.opcode} at position {CurrentPos}" );

		instruction.Invoke();
	}

	public MethodInfo FindOpcodeHandler( Opcode opcodeId )
	{
		var handlerAttribute = Assembly.GetExecutingAssembly().GetTypes()
					  .SelectMany( t => t.GetMethods() )
					  .Where( x => x.GetCustomAttribute<OpcodeHandlerAttribute>()?.Opcode == opcodeId )
					  .ToArray();

		return handlerAttribute.FirstOrDefault();
	}

	public void CallOpcodeHandler( Opcode opcodeId )
	{
		var handlerAttribute = FindOpcodeHandler( opcodeId );

		if ( handlerAttribute == null )
		{
			Log.Error( $"No handler for {opcodeId}, treating as no-op" );
			return;
		}

		handlerAttribute?.Invoke( null, null );
	}

	public void BranchTo( int value )
	{
		var destBranch = Branches.First( b => b.CompiledOffset == value );
		CurrentPos = destBranch.InstructionOffset;
	}
}
