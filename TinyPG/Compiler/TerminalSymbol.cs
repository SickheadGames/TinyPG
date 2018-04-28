// Copyright 2008 - 2010 Herre Kuijpers - <herre.kuijpers@gmail.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------
using System.Text;
using System.Text.RegularExpressions;

namespace TinyPG.Compiler
{
	public class TerminalSymbol : Symbol
	{
		public Regex Expression;

		public TerminalSymbol()
			: this("Terminal_" + ++counter, "")
		{ }

		public TerminalSymbol(string name)
			: this(name, "")
		{ }

		public TerminalSymbol(string name, string pattern)
		{
			Name = name;
			Expression = new Regex(pattern, RegexOptions.Compiled);
		}

		public TerminalSymbol(string name, Regex expression)
		{
			Name = name;
			Expression = expression;
		}

		public override string PrintProduction()
		{
			return Helper.Outline(Name, 0, " -> " + Expression + ";", 4);
		}
	}
}
