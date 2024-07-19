﻿using System.Numerics;
using Veldrid;

namespace OpenTPW;

internal class BaseTab
{
	public struct ConsoleItem
	{
		public Vector4 Color { get; set; }
		public string Text { get; set; }

		public ConsoleItem(Vector4 color, string text)
		{
			Color = color;
			Text = text;
		}
	}

	public ImGuiRenderer ImGuiRenderer { get; set; }

	public bool visible = false;

	public virtual void Draw()
	{
	}
}
