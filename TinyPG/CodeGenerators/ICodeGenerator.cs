using System;
using System.Collections.Generic;
using System.Text;

using TinyPG;
using TinyPG.Compiler;

namespace TinyPG.CodeGenerators
{
    public interface ICodeGenerator
    {
        /// <summary>
        /// the target filename where the output of Generate should be stored. This value
        /// must be implemented by the implementing class
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Generates an output file based on the grammar
        /// </summary>
        /// <param name="grammar">the grammar object model for the langauge</param>
        /// <param name="debug">a flag that indicates that the generated classes must implement the Debug intefaces (IParser, IParseTree or IToken). Default is false</param>
        /// <returns>returns the output classes to be stored in the output file</returns>
        string Generate(Grammar grammar, bool debug);
    }
}
