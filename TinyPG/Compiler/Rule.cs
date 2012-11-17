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
using System.Text.RegularExpressions;

namespace TinyPG.Compiler
{
    #region RuleType
    public enum RuleType
    {
        //Production = 0, // production rule
        /// <summary>
        /// represents a terminal symbol
        /// </summary>
        Terminal = 1,
        /// <summary>
        /// represents a non terminal symbol
        /// </summary>
        NonTerminal = 2,
        /// <summary>
        /// represents the | symbol, choose between one or the other symbol or subrule (OR)
        /// </summary>
        Choice = 3, // |
        /// <summary>
        /// puts two symbols or subrules in sequental order (AND)
        /// </summary>
        Concat = 4, // <whitespace>
        /// <summary>
        /// represents the ? symbol
        /// </summary>
        Option = 5, // ?
        /// <summary>
        /// represents the * symbol
        /// </summary>
        ZeroOrMore = 6, // *
        /// <summary>
        /// represents the + symbol
        /// </summary>
        OneOrMore = 7 // +
    }
    #endregion RuleType

    public class Rules : List<Rule>
    {
    }

    public class Rule
    {
        public Symbol Symbol;
        public Rules Rules;
        public RuleType Type;

        public Rule()
            : this(null, RuleType.Choice)
        {
        }
        
        public Rule(Symbol s) : this(s, s is TerminalSymbol ? RuleType.Terminal : RuleType.NonTerminal)
        {
        }
        
        public Rule(RuleType type) : this(null, type)
        {
        }
        
        public Rule(Symbol s, RuleType type)
        {
            Type = type; 
            Symbol = s;
            Rules = new Rules();
        }
        public Symbols GetFirstTerminals()
        {
            Symbols visited = new Symbols();
            Symbols FirstTerminals = new Symbols();
            DetermineFirstTerminals(FirstTerminals);
            return FirstTerminals;
        }


        public void DetermineProductionSymbols(Symbols symbols)
        {
            if (Type == RuleType.Terminal || Type == RuleType.NonTerminal)
            {
                symbols.Add(Symbol);
            }
            else
            {
                foreach (Rule rule in Rules)
                    rule.DetermineProductionSymbols(symbols);
            }

        }

        /*
        internal void DetermineLookAheadTree(LookAheadNode node)
        {
            switch (Type)
            {
                case RuleType.Terminal:
                    LookAheadNode f = node.Nodes.Find(Symbol.Name);
                    if (f == null)
                    {
                        LookAheadNode n = new LookAheadNode();
                        n.LookAheadTerminal = (TerminalSymbol) Symbol;
                        node.Nodes.Add(n);
                    }
                    else
                        Console.WriteLine("throw new Exception(\"Terminal already exists\");");
                    break;
                case RuleType.NonTerminal:
                    NonTerminalSymbol nts = Symbol as NonTerminalSymbol;

                    break;
                //case RuleType.Production:
                case RuleType.Concat:
                    break;
                case RuleType.OneOrMore:
                    break;
                case RuleType.Option:
                case RuleType.Choice:
                case RuleType.ZeroOrMore:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        */

        internal bool DetermineFirstTerminals(Symbols FirstTerminals)
        {
            return DetermineFirstTerminals(FirstTerminals, 0);
        }

        internal bool DetermineFirstTerminals(Symbols FirstTerminals, int index)
        {

            // indicates if Nonterminal can evaluate to an empty terminal (e.g. in case T -> a? or T -> a*)
            // in which case the parent rule should continue scanning after this nonterminal for Firsts.
            bool containsEmpty = false; // assume terminal is found
            switch (Type)
            {
                case RuleType.Terminal:
                    if (Symbol == null)
                        return true;

                        if (!FirstTerminals.Exists(Symbol))
                        FirstTerminals.Add(Symbol);
                    else
                        Console.WriteLine("throw new Exception(\"Terminal already exists\");");
                    break;
                case RuleType.NonTerminal:
                    if (Symbol == null)
                        return true;
                    
                    NonTerminalSymbol nts = Symbol as NonTerminalSymbol;                    
                    containsEmpty = nts.DetermineFirstTerminals();

                    // add first symbols of the nonterminal if not already added
                    foreach (TerminalSymbol t in nts.FirstTerminals)
                    {
                        if (!FirstTerminals.Exists(t))
                            FirstTerminals.Add(t);
                        else
                            Console.WriteLine("throw new Exception(\"Terminal already exists\");");
                    }
                    break;
                case RuleType.Choice:
                    {
                        // all subrules must be evaluated to determine if they contain first terminals
                        // if any subrule contains an empty, then this rule also contains an empty
                        foreach (Rule r in Rules)
                        {
                            containsEmpty |= r.DetermineFirstTerminals(FirstTerminals);
                        }
                        break;
                    }
                case RuleType.OneOrMore:
                    {
                        // if a non-empty subrule was found, then stop further parsing.
                        foreach (Rule r in Rules)
                        {
                            containsEmpty = r.DetermineFirstTerminals(FirstTerminals);
                            if (!containsEmpty) // found the final set of first terminals
                                break;
                        }
                        break;
                    }
                case RuleType.Concat:
                    {
                        // if a non-empty subrule was found, then stop further parsing.
                        // start scanning from Index

                        for (int i = index; i < Rules.Count; i++)
                        {
                            containsEmpty = Rules[i].DetermineFirstTerminals(FirstTerminals);
                            if (!containsEmpty) // found the final set of first terminals
                                break;
                        }

                        // assign this concat rule to each terminal
                        foreach (TerminalSymbol t in FirstTerminals)
                            t.Rule = this;

                        break;
                    }
                case RuleType.Option: 
                case RuleType.ZeroOrMore:
                    {
                        // empty due to the nature of this rule (A? or A* can always be empty)
                        containsEmpty = true;
                        
                        // if a non-empty subrule was found, then stop further parsing.
                        foreach (Rule r in Rules)
                        {
                            containsEmpty |= r.DetermineFirstTerminals(FirstTerminals);
                            if (!containsEmpty) // found the final set of first terminals
                                break;
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
            return containsEmpty;
        }

        public string PrintRule()
        {
            string r = "";

            switch (Type)
            {
                case RuleType.Terminal:
                case RuleType.NonTerminal:
                    if (Symbol != null)
                        r = Symbol.Name;
                    break;
                case RuleType.Concat:
                    foreach (Rule rule in Rules)
                    {
                        // continue recursively parsing all subrules
                        r += rule.PrintRule() + " ";
                    }
                    if (Rules.Count < 1)
                        r += " <- WARNING: ConcatRule contains no subrules";
                    break;
                case RuleType.Choice:
                    r += "(";
                    foreach (Rule rule in Rules)
                    {
                        if (r.Length > 1)
                            r += " | ";
                        // continue recursively parsing all subrules
                        r += rule.PrintRule();
                    }
                    r += ")";
                    if (Rules.Count < 1)
                        r += " <- WARNING: ChoiceRule contains no subrules";
                    break;
                case RuleType.ZeroOrMore:
                    if (Rules.Count >= 1)
                        r += "(" + Rules[0].PrintRule() + ")*";
                    if (Rules.Count > 1)
                        r += " <- WARNING: ZeroOrMoreRule contains more than 1 subrule";
                    if (Rules.Count < 1)
                        r += " <- WARNING: ZeroOrMoreRule contains no subrule";
                    break;
                case RuleType.OneOrMore:
                    if (Rules.Count >= 1)
                        r += "(" + Rules[0].PrintRule() + ")+";
                    if (Rules.Count > 1)
                        r += " <- WARNING: OneOrMoreRule contains more than 1 subrule";
                    if (Rules.Count < 1)
                        r += " <- WARNING: OneOrMoreRule contains no subrule";
                    break;
                case RuleType.Option:
                    if (Rules.Count >= 1)
                        r += "(" + Rules[0].PrintRule() + ")?";
                    if (Rules.Count > 1)
                        r += " <- WARNING: OptionRule contains more than 1 subrule";
                    if (Rules.Count < 1)
                        r += " <- WARNING: OptionRule contains no subrule";

                    break;
                default:
                    r = Symbol.Name;
                    break;
            }
            return r;
        }
    }
}
