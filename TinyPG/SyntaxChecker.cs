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
using System.Text;
using TinyPG.Controls;
using TinyPG.Compiler;

namespace TinyPG
{
	public sealed class SyntaxChecker : IDisposable
	{
		private TextMarker marker;
		private bool disposing;
		private string text;
		private bool textchanged;

		// used by the checker to check the syntax of the grammar while editing
		public ParseTree SyntaxTree { get; set; }

		// contains the runtime compiled grammar
		public Grammar Grammar { get; set; }

		public event EventHandler UpdateSyntax;

		public SyntaxChecker(TextMarker marker)
		{
			UpdateSyntax = null;
			this.marker = marker;
			disposing = false;
		}

		public void Start()
		{
			Scanner scanner = new Scanner();
			Parser parser = new Parser(scanner);

			while (!disposing)
			{
				System.Threading.Thread.Sleep(250);
				if (!textchanged)
					continue;

				textchanged = false;

				scanner.Init(text);
				SyntaxTree = parser.Parse(text, "", new GrammarTree());
				if (SyntaxTree.Errors.Count > 0)
					SyntaxTree.Errors.Clear();

				try
				{
					if (Grammar == null)
						Grammar = (Grammar)SyntaxTree.Eval();
					else
					{

						lock (Grammar)
						{
							Grammar = (Grammar)SyntaxTree.Eval();
						}
					}
				}
				catch (Exception)
				{

				}

				if (textchanged)
					continue;

				lock (marker)
				{
					marker.Clear();
					foreach (ParseError err in SyntaxTree.Errors)
					{
						marker.AddWord(err.Position, err.Length, System.Drawing.Color.Red, err.Message);
					}
				}

				if (UpdateSyntax != null)
					UpdateSyntax.Invoke(this, new EventArgs());
			}
		}

		public void Check(string text)
		{
			this.text = text;
			textchanged = true;
		}

		#region IDisposable Members

		public void Dispose()
		{
			disposing = true;
		}

		#endregion
	}
}
