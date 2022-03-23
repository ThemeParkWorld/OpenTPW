using StbImageSharp;
using System.Runtime.InteropServices;

namespace OpenTPW;

/// <summary>
/// Handles all processes relating to the window's icon.
/// </summary>
public static class IconLoader
{
    /// <summary>
	/// Load an image as an icon from a specified path.
	/// </summary>
    public static ReadOnlySpan<Silk.NET.Core.RawImage> LoadIcon( string path )
    {
        var fileData = File.ReadAllBytes( path );
        var image = ImageResult.FromMemory( fileData, ColorComponents.RedGreenBlueAlpha );

        var memory = new Memory<byte>( image.Data );
        var rawImage = new Silk.NET.Core.RawImage( image.Width, image.Height, memory );

        return new ReadOnlySpan<Silk.NET.Core.RawImage>( new[] { rawImage } );
    }
}
