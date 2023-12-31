using Veldrid;

namespace OpenTPW.ModKit;

internal interface IFileViewer
{
	TextureView GetIcon();
	void DrawPreview();
}
