using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenTPW
{
	public static class StringExtension
	{
		public static string ToSentenceCase( this string self )
		{
			return $"{char.ToUpper( self[0] )}{self.Substring( 1 )}";
		}
	}


	[Generator]
	public class IconsGenerator : ISourceGenerator
	{
		public void Execute( GeneratorExecutionContext context )
		{
			var sourceBuilder = new StringBuilder( @"
//
// Auto-generated code
//

using System;
namespace OpenTPW;

public static class Icons
{
" );

			var sourcePath = context.Compilation.Assembly.Locations.First().SourceTree.FilePath;
			var projectPath = sourcePath;

			for ( int i = 0; i < 4; ++i )
				projectPath = Directory.GetParent( projectPath ).FullName;

			var iconFiles = Directory.GetFiles( Path.Combine( projectPath, @"content/icons" ) );
			foreach ( var file in iconFiles )
			{
				var iconName = Path.GetFileNameWithoutExtension( file );
				// TODO: Relative path here
				sourceBuilder.Append( $"\tpublic static Texture {iconName.ToSentenceCase()} {{ get; set; }} = TextureBuilder.FromPath( @\"{file}\", false ).Build();\n" );
			}

			sourceBuilder.Append( @"}" );

			context.AddSource( "iconsGenerator.Generated.cs", SourceText.From( sourceBuilder.ToString(), Encoding.UTF8 ) );
		}

		public void Initialize( GeneratorInitializationContext context )
		{
		}
	}
}
