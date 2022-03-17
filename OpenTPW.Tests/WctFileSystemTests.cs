using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace OpenTPW.Tests;

[TestClass]
public class WctFileSystemTests
{
	[TestMethod]
	public void TestFileExists()
	{
		var fileSystem = new WadFileSystem( GameDir.GetPath( "data/ui.wad" ) );

		// Check for a file that should exist
		Assert.IsTrue( fileSystem.FileExists( "aerial.MD2" ) );

	}

	[TestMethod]
	public void TestFileNotExists()
	{
		var fileSystem = new WadFileSystem( GameDir.GetPath( "data/ui.wad" ) );

		// Check for a file that shouldn't exist
		Assert.IsFalse( fileSystem.FileExists( "non_existent_file.MD2" ) );
	}

	[TestMethod]
	public void TestFileOpen()
	{
		var expectedBytes = new byte[] { 70, 93, 209, 28 };

		var fileSystem = new WadFileSystem( GameDir.GetPath( "data/ui.wad" ) );
		var stream = fileSystem.OpenRead( "aerial.MD2" );

		var actualBytes = new byte[4];
		stream.Read( actualBytes, 0, 4 );
		Assert.IsTrue( Enumerable.SequenceEqual( actualBytes, expectedBytes ) );
	}

	[TestMethod]
	public void TestFileOpenNotExists()
	{
		var fileSystem = new WadFileSystem( GameDir.GetPath( "data/ui.wad" ) );
		Assert.ThrowsException<FileNotFoundException>( () => fileSystem.OpenRead( "non_existent_file.MD2" ) );
	}
}
