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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TinyPG.Controls
{
	public partial class AutoComplete : Form
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, char wParam, IntPtr lParam);

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;


		private const int WM_KEYDOWN = 0x100;
		private RichTextBox textEditor;

		// suppresses displaying the autocompletion screen while value > 0
		private int suppress;
		private int autocompletestart;

		// wordlist to show in the autocompletion list
		public ListBox WordList;

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			if (!Enabled)
				Visible = false;
		}

		public AutoComplete(RichTextBox editor)
		{
			this.textEditor = editor;
			this.textEditor.KeyDown += new KeyEventHandler(editor_KeyDown);
			this.textEditor.KeyUp += new KeyEventHandler(textEditor_KeyUp);
			this.textEditor.LostFocus += new EventHandler(textEditor_LostFocus);

			InitializeComponent();
		}

		void textEditor_LostFocus(object sender, EventArgs e)
		{
			if (textEditor.Focused || this.Focused || WordList.Focused) return;
			this.Visible = false;
		}

		void editor_KeyDown(object sender, KeyEventArgs e)
		{
			if (!this.Enabled) return;

			if (e.KeyValue == 32)
			{
				if (e.Control)
				{
					e.Handled = true;
					e.SuppressKeyPress = true;
				}

				if (suppress > 0)
					suppress--;
			}

			if (e.Control && e.KeyValue != 32)
				suppress = 2;

			if (e.KeyValue == 27 && this.Visible)
				suppress = 2;


			if (this.Visible)
			{
				// PgUp, PgDn, Up, Down
				if ((e.KeyValue == 33) || (e.KeyValue == 34) || (e.KeyValue == 38) || (e.KeyValue == 40))
				{
					this.SendKey((char)e.KeyValue);
					e.Handled = true;
				}
			}
		}

		void textEditor_KeyUp(object sender, KeyEventArgs e)
		{
			if (!this.Enabled) return;

			try
			{
				if ((e.KeyValue == 32 && !e.Control) || e.KeyValue == 13 || e.KeyValue == 27)
				{
					this.Visible = false;
				}
				else if ((((e.KeyValue > 64 && e.KeyValue < 91)) && !e.Control) || (e.KeyValue == 32 && e.Control))
				{
					if (!this.Visible)
					{

						int line = textEditor.GetFirstCharIndexOfCurrentLine();
						string t = Helper.Reverse(textEditor.Text.Substring(line, textEditor.SelectionStart - line));

						// scan the line of text for any of these characters. these mark the beginning of the word
						int i = t.IndexOfAny(" \r\n\t.;:\\/?><-=~`[]{}+!#$%^&*()".ToCharArray());
						if (i < 0) i = t.Length;
						autocompletestart = textEditor.SelectionStart - i;
						textEditor.Text.IndexOfAny(" \t\r\n".ToCharArray());
						Point p = textEditor.GetPositionFromCharIndex(autocompletestart);
						p = textEditor.PointToScreen(p);
						p.X -= 8;
						p.Y += 22;

						// only show autocompletion dialog if user has typed in the first characters, or if 
						// the user pressed CTRL-Space explicitly
						if (((textEditor.SelectionStart - autocompletestart) > 0 && (suppress <= 0)) || (e.KeyValue == 32 && e.Control))
						{
							this.Location = p;
							this.Visible = this.Enabled; // only display if enabled
							textEditor.Focus();
						}
					}

					//pre-select a word from the list that begins with the typed characters
					WordList.SelectedIndex = WordList.FindString(textEditor.Text.Substring(autocompletestart, textEditor.SelectionStart - autocompletestart));

				}
				else if (this.Visible)
				{
					if (e.KeyValue == 9 && !e.Alt && !e.Control && !e.Shift) // tab key
					{
						SelectCurrentWord();
						e.Handled = true;
					}

					if (textEditor.SelectionStart < autocompletestart)
						this.Visible = false;
					if ((e.KeyValue == 33) || (e.KeyValue == 34) || (e.KeyValue == 38) || (e.KeyValue == 40))
					{
						return;
					}
					if (this.Visible)
						WordList.SelectedIndex = WordList.FindString(textEditor.Text.Substring(autocompletestart, textEditor.SelectionStart - autocompletestart));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private void SelectCurrentWord()
		{
			this.Visible = false;
			if (this.WordList.SelectedItem == null)
				return;

			int temp = textEditor.SelectionStart;
			textEditor.Select(autocompletestart, temp - autocompletestart);
			textEditor.SelectedText = this.WordList.SelectedItem.ToString();
		}

		private void SendKey(char key)
		{
			SendMessage(WordList.Handle, WM_KEYDOWN, key, IntPtr.Zero);
		}

		void AutoComplete_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 32 || e.KeyValue == 27 || e.KeyValue == 13 || e.KeyValue == 9)
				this.Visible = false;

			if (e.KeyValue == 9 || e.KeyValue == 13)
				SelectCurrentWord();
		}

		// user selects a word using double click
		void WordList_DoubleClick(object sender, EventArgs e)
		{
			SelectCurrentWord();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.WordList = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// WordList
			// 
			this.WordList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.WordList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WordList.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.WordList.FormattingEnabled = true;
			this.WordList.ItemHeight = 15;
			this.WordList.Location = new System.Drawing.Point(0, 0);
			this.WordList.Name = "WordList";
			this.WordList.Size = new System.Drawing.Size(303, 137);
			this.WordList.Sorted = true;
			this.WordList.TabIndex = 0;
			this.WordList.UseTabStops = false;
			this.WordList.DoubleClick += new System.EventHandler(this.WordList_DoubleClick);
			// 
			// AutoComplete
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(303, 141);
			this.ControlBox = false;
			this.Controls.Add(this.WordList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.KeyPreview = true;
			this.Name = "AutoComplete";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TopMost = true;
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AutoComplete_KeyUp);
			this.ResumeLayout(false);

		}
	}
}
