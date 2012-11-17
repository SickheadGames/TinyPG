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

namespace TinyPG.Debug
{
    
    public interface IParseError
    {
        int Code { get; }
        int Line { get; }
        int Column { get; }
        int Position { get; }
        int Length { get; }
        string Message { get; }
    }

    public interface IParseTree : IParseNode
    {
        object Eval(params object[] paramlist);
        string PrintTree();
    }

    public interface IParseNode
    {
        IToken IToken { get; }
        List<IParseNode> INodes { get; }
        string Text { get; set; }
    }
}
