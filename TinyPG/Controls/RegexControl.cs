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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TinyPG.Controls
{
	public partial class RegExControl : UserControl
	{
		public RegExControl()
		{
			InitializeComponent();
		}

		private void textExpression_TextChanged(object sender, EventArgs e)
		{
			ValidateExpression();
		}

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			ValidateExpression();
		}

		private void textBox_Leave(object sender, EventArgs e)
		{
			ValidateExpression();
		}

		private void ValidateExpression()
		{
			// Suspend events, layout changes, and drawing updates.
			textBox.TextChanged -= textBox_TextChanged;
			textBox.SuspendLayout();
			DrawingControl.SuspendDrawing(textBox);

			try
			{
				// Save the original selection so we can restore it.
				var start = textBox.SelectionStart;
				var length = textBox.SelectionLength;

				// Clear the previous highlights.
				textBox.SelectAll();
				textBox.SelectionBackColor = Color.White;
				textBox.DeselectAll();

				textMatches.Text = "";

				RegexOptions options = (checkMultiline.Checked ? RegexOptions.Multiline : RegexOptions.Singleline);
				if (checkIgnoreCase.Checked) options = options | RegexOptions.IgnoreCase;

				Regex expr = new Regex(textExpression.Text, options);
				MatchCollection ms = expr.Matches(textBox.Text);

				statusText.Text = ms.Count + " match(es) found";

				StringBuilder sb = new StringBuilder();
				if (ms.Count > 0)
				{
					foreach (Match m in ms)
					{
						textBox.Select(m.Index, m.Length);
						textBox.SelectionBackColor = Color.LightPink;


						string[] names = expr.GetGroupNames();
						foreach (string group in names)
						{
							int val;
							if (int.TryParse(group, out val)) continue;

							sb.Append("<" + group + ">=");
							sb.Append(m.Groups[group].Value);
							sb.Append("\r\n");
						}

					}
				}

				// Restore the previous selection.
				textBox.Select(start, length);
				textMatches.Text = sb.ToString();
			}
			catch (Exception ex)
			{
				statusText.Text = ex.Message;
			}

			// Resume everything now that we're done.
			textBox.ResumeLayout();
			DrawingControl.ResumeDrawing(textBox);
			textBox.TextChanged += textBox_TextChanged;
		}

		private void checkIgnoreCase_CheckedChanged(object sender, EventArgs e)
		{
			ValidateExpression();
		}

		private void checkMultiline_CheckedChanged(object sender, EventArgs e)
		{
			ValidateExpression();
		}
	}
}
