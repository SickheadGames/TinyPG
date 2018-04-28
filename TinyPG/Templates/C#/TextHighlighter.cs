﻿// Automatically generated from source file: <%SourceFilename%>
// By TinyPG v1.3 available at http://github.com/SickheadGames/TinyPG


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;

namespace <%Namespace%>
{
	// Summary:
	//     System.EventArgs is the base class for classes containing event data.
	[Serializable]
	[ComVisible(true)]
	public class ContextSwitchEventArgs : EventArgs
	{
		public readonly ParseNode PreviousContext;
		public readonly ParseNode NewContext;

		// Summary:
		//     Initializes a new instance of the System.EventArgs class.
		public ContextSwitchEventArgs(ParseNode prevContext, ParseNode nextContext)
		{
			PreviousContext = prevContext;
			NewContext = nextContext;
		}
	}

	// delegate for firing context switch events
	public delegate void ContextSwitchEventHandler(object sender, ContextSwitchEventArgs e);

	/// <summary>
	/// Takes control over the RichTextBox and will color the text accoording to the rules of the parser and the scanner
	/// this control extender will also support Undo/Redo functionality.
	/// </summary>
	public class TextHighlighter : IDisposable
	{
		private class UndoItem
		{
			/// <summary>
			/// contains the information for an undo/redo action
			/// </summary>
			/// <param name="text">the full text to be undone/redone</param>
			/// <param name="position">position of the caret after the un/redo action</param>
			/// <param name="scroll">position of the scrollbars after un/redo action</param>
			public UndoItem(string text, int position, Point scroll)
			{
				Text = text;
				Position = position;
				ScrollPosition = scroll;
			}

			public string Text;
			public int Position;
			public Point ScrollPosition;
		}


		// some winapís required
		[DllImport("user32", CharSet = CharSet.Auto)]
		private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern bool PostMessageA(IntPtr hWnd, int nBar, int wParam, int lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetScrollPos(int hWnd, int nBar);

		[DllImport("user32.dll")]
		private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

		private const int WM_SETREDRAW = 0x000B;
		private const int WM_USER = 0x400;
		private const int EM_GETEVENTMASK = (WM_USER + 59);
		private const int EM_SETEVENTMASK = (WM_USER + 69);
		private const int SB_HORZ = 0x0;
		private const int SB_VERT = 0x1;
		private const int WM_HSCROLL = 0x114;
		private const int WM_VSCROLL = 0x115;
		private const int SB_THUMBPOSITION = 4;
		private const int UNDO_BUFFER = 100;

		private int HScrollPos
		{
			get { return GetScrollPos((int)Textbox.Handle, SB_HORZ); }
			set
			{
				SetScrollPos((IntPtr)Textbox.Handle, SB_HORZ, value, true);
				PostMessageA((IntPtr)Textbox.Handle, WM_HSCROLL, SB_THUMBPOSITION + 0x10000 * value, 0);
			}
		}

		private int VScrollPos
		{
			get { return GetScrollPos((int)Textbox.Handle, SB_VERT); }
			set
			{
				SetScrollPos((IntPtr)Textbox.Handle, SB_VERT, value, true);
				PostMessageA((IntPtr)Textbox.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * value, 0);
			}
		}

		// public shared members
		public ParseTree Tree;
		public readonly RichTextBox Textbox;

		// private members
		private Parser Parser;
		private Scanner Scanner;
		private IntPtr stateLocked = IntPtr.Zero;

		private int UndoIndex = -1;
		private List<UndoItem> UndoList;

		private ParseNode currentContext;
		public event ContextSwitchEventHandler SwitchContext;

		private Thread threadAutoHighlight;


		private void Do(string text, int position)
		{
            
			if (stateLocked != IntPtr.Zero) return;

			UndoItem ua = new UndoItem(text, position, new Point(HScrollPos, VScrollPos));
			UndoList.RemoveRange(UndoIndex, UndoList.Count - UndoIndex);
			UndoList.Add(ua);
			if (UndoList.Count > UNDO_BUFFER)
				UndoList.RemoveAt(0);

			// make undo/redo a little smarter, remove single strokes
			// reducing nr of undo states
			if (UndoList.Count > 7)
			{
				bool canRemove = true;
				UndoItem nextItem = ua;
				for (int i = 0; i < 6; i++)
				{
					UndoItem prevItem = UndoList[UndoList.Count - 2 - i];
					canRemove &= (Math.Abs(prevItem.Text.Length - nextItem.Text.Length) <= 1 && Math.Abs(prevItem.Position - nextItem.Position) <= 1);
					nextItem = prevItem;
				}
				if (canRemove)
				{
					UndoList.RemoveRange(UndoList.Count - 6, 5);
				}
			}
			UndoIndex = UndoList.Count;
		}

		public void ClearUndo()
		{
			UndoList = new List<UndoItem>();
			UndoIndex = 0;
		}

		public void Undo()
		{
			if (!CanUndo) return;

			UndoIndex--;
			if (UndoIndex < 1)
				UndoIndex = 1;

			// implement undo action here
			UndoItem ua = UndoList[UndoIndex-1];
			RestoreState(ua);
		}

		public void Redo()
		{
			if (!CanRedo) return;

			UndoIndex++;
			if (UndoIndex > UndoList.Count)
				UndoIndex = UndoList.Count;

			UndoItem ua = UndoList[UndoIndex-1];
			RestoreState(ua);

		}

		private void RestoreState(UndoItem item)
		{
			Lock();
			// restore state
			Textbox.Rtf = item.Text;
			Textbox.Select(item.Position, 0);
			HScrollPos = item.ScrollPosition.X;
			VScrollPos = item.ScrollPosition.Y;
            
			Unlock();
		}

		public bool CanUndo
		{
			get { return UndoIndex > 0; }
		}

		public bool CanRedo
		{
			get { return UndoIndex < UndoList.Count; }
		}

		public TextHighlighter(RichTextBox textbox, Scanner scanner, Parser parser)
		{
			Textbox = textbox;
			Scanner = scanner;
			Parser = parser;

			ClearUndo();

			//Tree = Parser.Parse(Textbox.Text);
			Textbox.TextChanged += new EventHandler(Textbox_TextChanged);
			textbox.KeyDown += new KeyEventHandler(textbox_KeyDown);
			Textbox.SelectionChanged += new EventHandler(Textbox_SelectionChanged);
			Textbox.Disposed += new EventHandler(Textbox_Disposed);

			SwitchContext = null;
			currentContext = Tree;

			threadAutoHighlight = new Thread(AutoHighlightStart);
			threadAutoHighlight.Start();
		}


		public void Lock()
		{
			// Stop redrawing:  
			SendMessage(Textbox.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
			// Stop sending of events:  
			stateLocked = SendMessage(Textbox.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);
			// change colors and stuff in the RichTextBox  
		}

		public void Unlock()
		{
			// turn on events  
			SendMessage(Textbox.Handle, EM_SETEVENTMASK, 0, stateLocked);
			// turn on redrawing  
			SendMessage(Textbox.Handle, WM_SETREDRAW, 1, IntPtr.Zero);

			stateLocked = IntPtr.Zero;
			Textbox.Invalidate();
		}
  
		void textbox_KeyDown(object sender, KeyEventArgs e)
		{
			// undo/redo
			if (e.KeyValue == 89 && e.Control) // CTRL-Y
				Redo();
			if (e.KeyValue == 90 && e.Control) // CTRL-Z
				Undo();
		}

		void Textbox_TextChanged(object sender, EventArgs e)
		{
			if (stateLocked != IntPtr.Zero) return;

			Do(Textbox.Rtf, Textbox.SelectionStart);
            
			HighlightText();
		}

		void Textbox_SelectionChanged(object sender, EventArgs e)
		{
			if (stateLocked != IntPtr.Zero) return;

			if (SwitchContext == null) return;
			ParseNode newContext = GetCurrentContext();
            
			if (currentContext == null) 
				currentContext = newContext;
			if (newContext == null) return;

			if (newContext.Token.Type != currentContext.Token.Type)
			{
				SwitchContext.Invoke(this, new ContextSwitchEventArgs(currentContext, newContext));
				currentContext = newContext;
			}
            
		}

		/// <summary>
		/// this handy function returns the section in which the user is editing currently
		/// </summary>
		/// <returns></returns>
		public ParseNode GetCurrentContext()
		{
			ParseNode node = FindNode(Tree, Textbox.SelectionStart);
			return node;
		}

		private ParseNode FindNode(ParseNode node, int posstart)
		{
			if (node == null) return null;

			if (node.Token.StartPos <= posstart && (node.Token.StartPos + node.Token.Length) >= posstart)
			{
				foreach (ParseNode n in node.Nodes)
				{
					if (n.Token.StartPos <= posstart && (n.Token.StartPos + n.Token.Length) >= posstart)
						return FindNode(n, posstart);
				}
				return node;
			}
			else
				return null;
		}

		/// <summary>
		/// use HighlighText to start the text highlight process from the caller's thread.
		/// this method is not used internally. 
		/// </summary>
		public void HighlightText()
		{
			lock (treelock)
			{
				textChanged = true;
				currentText = Textbox.Text;
			}
		}

		// highlight the text (used internally only)
		private void HighlightTextInternal()
		{
			Lock();

			int hscroll = HScrollPos;
			int vscroll = VScrollPos;

			int selstart = Textbox.SelectionStart;

			HighlighTextCore();

			Textbox.Select(selstart,0);

			HScrollPos = hscroll;
			VScrollPos = vscroll;

			Unlock();
		}

		/// <summary>
		/// this method should be used only by HighlightText or RestoreState methods
		/// </summary>
		private void HighlighTextCore()
		{
			//Tree = Parser.Parse(Textbox.Text);
			StringBuilder sb = new StringBuilder();
			if (Tree == null) return;

			ParseNode start = Tree.Nodes[0];
			HightlightNode(start, sb);

			// append any trailing skipped tokens that were scanned
			foreach (Token skiptoken in Scanner.Skipped)
			{
				HighlightToken(skiptoken, sb);
				sb.Append(skiptoken.Text.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}").Replace("\n", "\\par\n"));
			}

			// NOTE: if you do not need unicode characters and you need a little bit more performance
			// you can comment out the following line:
			sb = Unicode(sb);     // <--- without this, unicode characters will be garbled after highlighting

			AddRtfHeader(sb);
			AddRtfEnd(sb);

			Textbox.Rtf = sb.ToString();

		}

				/// <summary>
		/// added function to convert unicode characters in the stringbuilder to rtf unicode escapes
		/// </summary>
		public StringBuilder Unicode(StringBuilder sb)
		{
			int i = 0;
			StringBuilder uc = new StringBuilder();
			for (i = 0; i <= sb.Length - 1; i++)
			{
				char c = sb[i];
                
				if ((int)c < 127)
				{
					uc.Append(c);
				}
				else
				{
					uc.Append("\\u" + ((int)c).ToString() + "?");
				}
			}
			return uc;
		}

		// thread start for the automatic highlighting
		private static object treelock = new object();
		private bool isDisposing;
		private bool textChanged;
		private string currentText;

		private void AutoHighlightStart()
		{
			ParseTree _tree;
			string _currenttext = "";
			while (!isDisposing)
			{
				bool _textchanged;
				lock (treelock)
				{
					_textchanged = textChanged;
					if (textChanged)
					{
						textChanged = false;
						_currenttext = currentText;
					}
				}
				if (!_textchanged)
				{
					Thread.Sleep(200);
					continue;
				}

				_tree = (ParseTree)Parser.Parse(_currenttext);
				lock (treelock)
				{
					if (textChanged)
						continue;
					else
						Tree = _tree; // assign new tree
				}
                
				Textbox.Invoke(new MethodInvoker(HighlightTextInternal));

			}
		}


		/// <summary>
		/// inserts the RTF codes to highlight text blocks
		/// </summary>
		/// <param name="node">the node to highlight, will be appended to sb</param>
		/// <param name="sb">the final output string</param>
		private void HightlightNode(ParseNode node, StringBuilder sb)
		{
			if (node.Nodes.Count == 0)
			{
				if (node.Token.Skipped != null)
				{
					foreach(Token skiptoken in node.Token.Skipped)
					{
						HighlightToken(skiptoken, sb);
						sb.Append(skiptoken.Text.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}").Replace("\n", "\\par\n"));
					}
				}
				HighlightToken(node.Token, sb);
				sb.Append(node.Token.Text.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}").Replace("\n", "\\par\n"));
				sb.Append(@"}");
			}

			foreach (ParseNode n in node.Nodes)
			{
				HightlightNode(n, sb);
			}
		}

				/// <summary>
		/// inserts the RTF codes to highlight text blocks
		/// </summary>
		/// <param name="token">the token to highlight, will be appended to sb</param>
		/// <param name="sb">the final output string</param>
		private void HighlightToken(Token token, StringBuilder sb)
		{
			switch (token.Type)
			{
<%HightlightTokens%>
				default:
					sb.Append(@"{{\cf0 ");
					break;
			}
		}

		// define the color palette to be used here
		private void AddRtfHeader(StringBuilder sb)
		{
			sb.Insert(0, @"{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 Consolas;}}{\colortbl;<%RtfColorPalette%>}\viewkind4\uc1\pard\lang1033\f0\fs20");
		}

		private void AddRtfEnd(StringBuilder sb)
		{
			sb.Append("}");
		}

		void Textbox_Disposed(object sender, EventArgs e)
		{
			Dispose();
		}

		#region IDisposable Members

		public void Dispose()
		{
			isDisposing = true;
			threadAutoHighlight.Join(1000);
			if (threadAutoHighlight.IsAlive)
				threadAutoHighlight.Abort();
		}

		#endregion
	}
}
