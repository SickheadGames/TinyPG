// Copyright 2008 - 2010 Herre Kuijpers - <herre.kuijpers@gmail.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TinyPG.Controls
{
	/// <summary>
	/// the text marker is responsible for underlining erronious text (= marked words) with a wavy line
	/// the text marker also handles the display of the tooltip
	/// </summary>
	public sealed class TextMarker : NativeWindow, IDisposable
	{
		public RichTextBox Textbox;
		private List<Word> MarkedWords;
		private ToolTip ToolTip;
		private Point lastMousePos;

		private struct Word
		{
			public int Start;
			public int Length;
			public Color Color;
			public string ToolTip;
		}

		public TextMarker(RichTextBox textbox)
		{
			Textbox = textbox;
			Textbox.MouseMove += new MouseEventHandler(Textbox_MouseMove);
			this.AssignHandle(Textbox.Handle);
			ToolTip = new ToolTip();
			Clear();
			lastMousePos = new Point();
		}

		void Textbox_MouseMove(object sender, MouseEventArgs e)
		{
			if (lastMousePos.X == e.X || lastMousePos.Y == e.Y)
				return;

			lastMousePos = new Point(e.X, e.Y);
			int i = Textbox.GetCharIndexFromPosition(lastMousePos);

			bool found = false;
			foreach (Word w in MarkedWords)
			{
				if (w.Start <= i && w.Start + w.Length > i)
				{
					Point p = Textbox.GetPositionFromCharIndex(w.Start);
					p.Y += 18;

					ToolTip.Show(w.ToolTip, (IWin32Window)Textbox, p);
					found = true;
				}
			}

			if (!found)
			{
				ToolTip.Hide((IWin32Window)Textbox);
			}
		}

		protected override void WndProc(ref Message m)
		{
			// pre-process the text control's messages
			switch (m.Msg)
			{
				case 0x14: // WM_ERASEBKGND
					base.WndProc(ref m);
					MarkWords();
					break;
				case 0x114: // WM_HSCROLL
					base.WndProc(ref m);
					MarkWords();
					break;
				case 0x115: // WM_VSCROLL
					base.WndProc(ref m);
					MarkWords();
					break;
				case 0x0101: // WM_KEYUP
					base.WndProc(ref m);
					MarkWords();
					break;
				case 0x113: // WM_TIMER
					base.WndProc(ref m);
					MarkWords();
					break;
				default:
					//Console.WriteLine(m.Msg);
					base.WndProc(ref m);
					break;
			}
		}

		public void AddWord(int wordstart, int wordlen, Color color)
		{
			AddWord(wordstart, wordlen, color, "");
		}

		public void AddWord(int wordstart, int wordlen, Color color, string ToolTip)
		{
			Word word = new Word();
			word.Start = wordstart;
			word.Length = wordlen;
			word.Color = color;
			word.ToolTip = ToolTip;
			MarkedWords.Add(word);
		}

		public void Clear()
		{
			MarkedWords = new List<Word>();
		}

		public void MarkWords()
		{
			if (Textbox.IsDisposed || !Textbox.Enabled || !Textbox.Visible) return;

			Graphics graphics = Textbox.CreateGraphics();

			int minpos = Textbox.GetCharIndexFromPosition(new Point(0, 0));
			int maxpos = Textbox.GetCharIndexFromPosition(new Point(Textbox.Width, Textbox.Height));
			foreach (Word w in MarkedWords)
			{
				// check if the marked word is currently displayed on screen
				if (w.Start + w.Length < minpos || w.Start > maxpos)
					continue;

				MarkWord(w, graphics);
			}
			graphics.Dispose();
		}

		private void MarkWord(Word word, Graphics graphics)
		{
			GraphicsPath path = new GraphicsPath();

			List<Point> points = new List<Point>();
			Point p1 = Textbox.GetPositionFromCharIndex(word.Start);
			Point p2 = Textbox.GetPositionFromCharIndex(word.Start + word.Length);

			if (word.Length == 0)
			{
				p1.X -= 5;
				p2.X += 5;
			}

			p1.Y += Textbox.Font.Height - 2;
			points.Add(p1);
			bool up = true;
			for (int x = p1.X + 2; x < p2.X + 2; x += 2)
			{
				Point p = up ? new Point(x, p1.Y + 2) : new Point(x, p1.Y);
				points.Add(p);
				up = !up;
			}
			if (points.Count > 1)
			{
				path.StartFigure();
				path.AddLines(points.ToArray());
			}

			Pen pen = new Pen(word.Color);
			graphics.DrawPath(pen, path);
			pen.Dispose();
			path.Dispose();
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.ReleaseHandle();
		}

		#endregion
	}
}
