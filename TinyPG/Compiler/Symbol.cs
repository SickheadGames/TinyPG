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

namespace TinyPG.Compiler
{
    public class Symbols : List<Symbol>
    {
        public bool Exists(Symbol symbol)
        {
            return this.Exists(new Predicate<Symbol>(delegate(Symbol s) { return s.Name == symbol.Name; }));
        }

        public Symbol Find(string Name)
        {
            return this.Find(delegate(Symbol s) { return s == null ? false : s.Name == Name; });
        }
    }

    // allows assigning attributes to the node
    public class SymbolAttributes : Dictionary<string, object[]>
    {
    }

    public abstract class Symbol
    {
        public SymbolAttributes Attributes;

        protected static int counter = 0;

        // the name of the symbol
        public string Name;

        // an attached piece of sourcecode
        public string CodeBlock;

        public Rule Rule; // the rule this symbol is used in.

        public abstract string PrintProduction();

        protected Symbol()
        {
            Attributes = new SymbolAttributes();
        }
    }
}
