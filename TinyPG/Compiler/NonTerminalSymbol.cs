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
    public class NonTerminalSymbol : Symbol
    {
        public Rules Rules;

        // indicates if Nonterminal can evaluate to an empty terminal (e.g. in case T -> a? or T -> a*)
        // in which case the parent rule should continue scanning after this nonterminal for Firsts.
        private bool containsEmpty;
        private int visitCount;
        public Symbols FirstTerminals;

        public NonTerminalSymbol()
            : this("NTS_" + ++counter)
        {
        }

        public NonTerminalSymbol(string name)
        {
            FirstTerminals = new Symbols();
            Rules = new Rules();
            Name = name;
            containsEmpty = false;
            visitCount = 0;
        }

        /*
        internal void DetermineLookAheadTree(LookAheadNode node)
        {
            //recursion here
            foreach (Rule rule in Rules)
            {
                rule.DetermineLookAheadTree(node);
            }
        }
        */
        internal bool DetermineFirstTerminals()
        {
            // check if nonterminal has already been visited x times
            // only determine firsts x times to allow for recursion of depth x, otherwise may wind up in endless loop
            if (visitCount > 10) 
                return containsEmpty;

            visitCount++;

            // reset terminals
            FirstTerminals = new Symbols();

            //recursion here
            
            foreach (Rule rule in Rules)
            {
                containsEmpty |= rule.DetermineFirstTerminals(FirstTerminals);
            }
            return containsEmpty;
        }

        /// <summary>
        /// returns a list of symbols used by this production
        /// </summary>
        public Symbols DetermineProductionSymbols()
        {
            Symbols symbols = new Symbols();
            foreach (Rule rule in Rules)
                rule.DetermineProductionSymbols(symbols);

            return symbols;
        }

        public override string PrintProduction()
        {
            string p = "";
            foreach (Rule r in Rules)
            {
                p += r.PrintRule() + ";";
            }

            return Helper.Outline(Name, 0, " -> " + p, 4);
        }

    }
}
