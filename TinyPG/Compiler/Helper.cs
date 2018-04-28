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

// extends the System.Text namespace
namespace System.Text
{
	/// <summary>
	/// some handy to use static helper functions
	/// Note that this class is used by the TinyTG.Compiler classes for string formatting
	/// </summary>
	public sealed class Helper
	{
		private Helper()
		{
		}

		public static string Reverse(string text)
		{
			char[] charArray = new char[text.Length];
			int len = text.Length - 1;
			for (int i = 0; i <= len; i++)
				charArray[i] = text[len - i];
			return new string(charArray);
		}

		public static string Outline(string text1, int indent1, string text2, int indent2)
		{
			string r = Indent(indent1);
			r += text1;
			r = r.PadRight((indent2 * 4) % 256, ' ');
			r += text2;
			return r;
		}

		public static string Indent(int indentcount)
		{
			string t = "";
			for (int i = 0; i < indentcount; i++)
				t += "    ";

			return t;
		}

		/// <summary>
		/// will add a comment that can be used for debugging problems in generated code
		/// comment will only be added if profile is set to Debug mode
		/// </summary>
		/// <param name="comment">the comment to write to the file</param>
		/// <returns></returns>
		public static string AddComment(string comment)
		{
			return AddComment("//", comment);
		}

		public static string AddComment(string commenter, string comment)
		{
#if DEBUG
			return " " + commenter + " " + comment;
#else
            return "";
#endif
		}
	}
}
