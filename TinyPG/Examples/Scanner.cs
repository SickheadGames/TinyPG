// Automatically generated from source file: C:\Users\Ping\Documents\Visual Studio 2015\Projects\TinyPG_gitmine\TinyPG\Examples\tiny_language2.tpg
// By TinyPG v1.3 available at http://github.com/SickheadGames/TinyPG

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace TinyExe
{
    #region Scanner

    public partial class Scanner
    {
        public string Input;
        public int StartPos = 0;
        public int EndPos = 0;
        public string CurrentFile;
        public int CurrentLine;
        public int CurrentColumn;
        public int CurrentPosition;
        public List<Token> Skipped; // tokens that were skipped
        public Dictionary<TokenType, Regex> Patterns;

        private Token LookAheadToken;
        private List<TokenType> Tokens;
        private List<TokenType> SkipList; // tokens to be skipped
        private readonly TokenType FileAndLine;

        public Scanner()
        {
            Regex regex;
            Patterns = new Dictionary<TokenType, Regex>();
            Tokens = new List<TokenType>();
            LookAheadToken = null;
            Skipped = new List<Token>();

            SkipList = new List<TokenType>();
            SkipList.Add(TokenType.WHITESPACE);

            regex = new Regex(@"true|false", RegexOptions.Compiled);
            Patterns.Add(TokenType.BOOLEANLITERAL, regex);
            Tokens.Add(TokenType.BOOLEANLITERAL);

            regex = new Regex(@"[0-9]+(UL|Ul|uL|ul|LU|Lu|lU|lu|U|u|L|l)?", RegexOptions.Compiled);
            Patterns.Add(TokenType.DECIMALINTEGERLITERAL, regex);
            Tokens.Add(TokenType.DECIMALINTEGERLITERAL);

            regex = new Regex(@"([0-9]+\.[0-9]+([eE][+-]?[0-9]+)?([fFdDMm]?)?)|(\.[0-9]+([eE][+-]?[0-9]+)?([fFdDMm]?)?)|([0-9]+([eE][+-]?[0-9]+)([fFdDMm]?)?)|([0-9]+([fFdDMm]?))", RegexOptions.Compiled);
            Patterns.Add(TokenType.REALLITERAL, regex);
            Tokens.Add(TokenType.REALLITERAL);

            regex = new Regex(@"0(x|X)[0-9a-fA-F]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.HEXINTEGERLITERAL, regex);
            Tokens.Add(TokenType.HEXINTEGERLITERAL);

            regex = new Regex(@"\""(\""\""|[^\""])*\""", RegexOptions.Compiled);
            Patterns.Add(TokenType.STRINGLITERAL, regex);
            Tokens.Add(TokenType.STRINGLITERAL);

            regex = new Regex(@"[a-zA-Z_][a-zA-Z0-9_]*(?=\s*\()", RegexOptions.Compiled);
            Patterns.Add(TokenType.FUNCTION, regex);
            Tokens.Add(TokenType.FUNCTION);

            regex = new Regex(@"[a-zA-Z_][a-zA-Z0-9_]*(?!\s*\()", RegexOptions.Compiled);
            Patterns.Add(TokenType.VARIABLE, regex);
            Tokens.Add(TokenType.VARIABLE);

            regex = new Regex(@"(?i)pi|e", RegexOptions.Compiled);
            Patterns.Add(TokenType.CONSTANT, regex);
            Tokens.Add(TokenType.CONSTANT);

            regex = new Regex(@"{\s*", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRACEOPEN, regex);
            Tokens.Add(TokenType.BRACEOPEN);

            regex = new Regex(@"\s*}", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRACECLOSE, regex);
            Tokens.Add(TokenType.BRACECLOSE);

            regex = new Regex(@"\(\s*", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRACKETOPEN, regex);
            Tokens.Add(TokenType.BRACKETOPEN);

            regex = new Regex(@"\s*\)", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRACKETCLOSE, regex);
            Tokens.Add(TokenType.BRACKETCLOSE);

            regex = new Regex(@";", RegexOptions.Compiled);
            Patterns.Add(TokenType.SEMICOLON, regex);
            Tokens.Add(TokenType.SEMICOLON);

            regex = new Regex(@"\+\+", RegexOptions.Compiled);
            Patterns.Add(TokenType.PLUSPLUS, regex);
            Tokens.Add(TokenType.PLUSPLUS);

            regex = new Regex(@"--", RegexOptions.Compiled);
            Patterns.Add(TokenType.MINUSMINUS, regex);
            Tokens.Add(TokenType.MINUSMINUS);

            regex = new Regex(@"\|\|", RegexOptions.Compiled);
            Patterns.Add(TokenType.PIPEPIPE, regex);
            Tokens.Add(TokenType.PIPEPIPE);

            regex = new Regex(@"&&", RegexOptions.Compiled);
            Patterns.Add(TokenType.AMPAMP, regex);
            Tokens.Add(TokenType.AMPAMP);

            regex = new Regex(@"&(?!&)", RegexOptions.Compiled);
            Patterns.Add(TokenType.AMP, regex);
            Tokens.Add(TokenType.AMP);

            regex = new Regex(@"\^", RegexOptions.Compiled);
            Patterns.Add(TokenType.POWER, regex);
            Tokens.Add(TokenType.POWER);

            regex = new Regex(@"\+", RegexOptions.Compiled);
            Patterns.Add(TokenType.PLUS, regex);
            Tokens.Add(TokenType.PLUS);

            regex = new Regex(@"-", RegexOptions.Compiled);
            Patterns.Add(TokenType.MINUS, regex);
            Tokens.Add(TokenType.MINUS);

            regex = new Regex(@"=", RegexOptions.Compiled);
            Patterns.Add(TokenType.EQUAL, regex);
            Tokens.Add(TokenType.EQUAL);

            regex = new Regex(@":=", RegexOptions.Compiled);
            Patterns.Add(TokenType.ASSIGN, regex);
            Tokens.Add(TokenType.ASSIGN);

            regex = new Regex(@"!=|<>", RegexOptions.Compiled);
            Patterns.Add(TokenType.NOTEQUAL, regex);
            Tokens.Add(TokenType.NOTEQUAL);

            regex = new Regex(@"!", RegexOptions.Compiled);
            Patterns.Add(TokenType.NOT, regex);
            Tokens.Add(TokenType.NOT);

            regex = new Regex(@"\*", RegexOptions.Compiled);
            Patterns.Add(TokenType.ASTERIKS, regex);
            Tokens.Add(TokenType.ASTERIKS);

            regex = new Regex(@"/", RegexOptions.Compiled);
            Patterns.Add(TokenType.SLASH, regex);
            Tokens.Add(TokenType.SLASH);

            regex = new Regex(@"%", RegexOptions.Compiled);
            Patterns.Add(TokenType.PERCENT, regex);
            Tokens.Add(TokenType.PERCENT);

            regex = new Regex(@"\?", RegexOptions.Compiled);
            Patterns.Add(TokenType.QUESTIONMARK, regex);
            Tokens.Add(TokenType.QUESTIONMARK);

            regex = new Regex(@",", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMMA, regex);
            Tokens.Add(TokenType.COMMA);

            regex = new Regex(@"<=", RegexOptions.Compiled);
            Patterns.Add(TokenType.LESSEQUAL, regex);
            Tokens.Add(TokenType.LESSEQUAL);

            regex = new Regex(@">=", RegexOptions.Compiled);
            Patterns.Add(TokenType.GREATEREQUAL, regex);
            Tokens.Add(TokenType.GREATEREQUAL);

            regex = new Regex(@"<(?!>)", RegexOptions.Compiled);
            Patterns.Add(TokenType.LESSTHAN, regex);
            Tokens.Add(TokenType.LESSTHAN);

            regex = new Regex(@">", RegexOptions.Compiled);
            Patterns.Add(TokenType.GREATERTHAN, regex);
            Tokens.Add(TokenType.GREATERTHAN);

            regex = new Regex(@":", RegexOptions.Compiled);
            Patterns.Add(TokenType.COLON, regex);
            Tokens.Add(TokenType.COLON);

            regex = new Regex(@"^$", RegexOptions.Compiled);
            Patterns.Add(TokenType.EOF, regex);
            Tokens.Add(TokenType.EOF);

            regex = new Regex(@"\s+", RegexOptions.Compiled);
            Patterns.Add(TokenType.WHITESPACE, regex);
            Tokens.Add(TokenType.WHITESPACE);


        }

        public void Init(string input)
        {
            Init(input, "");
        }

        public void Init(string input, string fileName)
        {
            this.Input = input;
            StartPos = 0;
            EndPos = 0;
            CurrentFile = fileName;
            CurrentLine = 1;
            CurrentColumn = 1;
            CurrentPosition = 0;
            LookAheadToken = null;
        }

        public Token GetToken(TokenType type)
        {
            Token t = new Token(this.StartPos, this.EndPos);
            t.Type = type;
            return t;
        }

         /// <summary>
        /// executes a lookahead of the next token
        /// and will advance the scan on the input string
        /// </summary>
        /// <returns></returns>
        public Token Scan(params TokenType[] expectedtokens)
        {
            Token tok = LookAhead(expectedtokens); // temporarely retrieve the lookahead
            LookAheadToken = null; // reset lookahead token, so scanning will continue
            StartPos = tok.EndPos;
            EndPos = tok.EndPos; // set the tokenizer to the new scan position
            CurrentLine = tok.Line + (tok.Text.Length - tok.Text.Replace("\n", "").Length);
            CurrentFile = tok.File;
            return tok;
        }

        /// <summary>
        /// returns token with longest best match
        /// </summary>
        /// <returns></returns>
        public Token LookAhead(params TokenType[] expectedtokens)
        {
            int i;
            int startpos = StartPos;
            int endpos = EndPos;
            int currentline = CurrentLine;
            string currentFile = CurrentFile;
            Token tok = null;
            List<TokenType> scantokens;


            // this prevents double scanning and matching
            // increased performance
            if (LookAheadToken != null 
                && LookAheadToken.Type != TokenType._UNDETERMINED_ 
                && LookAheadToken.Type != TokenType._NONE_) return LookAheadToken;

            // if no scantokens specified, then scan for all of them (= backward compatible)
            if (expectedtokens.Length == 0)
                scantokens = Tokens;
            else
            {
                scantokens = new List<TokenType>(expectedtokens);
                scantokens.AddRange(SkipList);
            }

            do
            {

                int len = -1;
                TokenType index = (TokenType)int.MaxValue;
                string input = Input.Substring(startpos);

                tok = new Token(startpos, endpos);

                for (i = 0; i < scantokens.Count; i++)
                {
                    Regex r = Patterns[scantokens[i]];
                    Match m = r.Match(input);
                    if (m.Success && m.Index == 0 && ((m.Length > len) || (scantokens[i] < index && m.Length == len )))
                    {
                        len = m.Length;
                        index = scantokens[i];  
                    }
                }

                if (index >= 0 && len >= 0)
                {
                    tok.EndPos = startpos + len;
                    tok.Text = Input.Substring(tok.StartPos, len);
                    tok.Type = index;
                }
                else if (tok.StartPos == tok.EndPos)
                {
                    if (tok.StartPos < Input.Length)
                        tok.Text = Input.Substring(tok.StartPos, 1);
                    else
                        tok.Text = "EOF";
                }

                // Update the line and column count for error reporting.
                tok.File = currentFile;
                tok.Line = currentline;
                if (tok.StartPos < Input.Length)
                    tok.Column = tok.StartPos - Input.LastIndexOf('\n', tok.StartPos);

                if (SkipList.Contains(tok.Type))
                {
                    startpos = tok.EndPos;
                    endpos = tok.EndPos;
                    currentline = tok.Line + (tok.Text.Length - tok.Text.Replace("\n", "").Length);
                    currentFile = tok.File;
                    Skipped.Add(tok);
                }
                else
                {
                    // only assign to non-skipped tokens
                    tok.Skipped = Skipped; // assign prior skips to this token
                    Skipped = new List<Token>(); //reset skips
                }

                // Check to see if the parsed token wants to 
                // alter the file and line number.
                if (tok.Type == FileAndLine)
                {
                    Match match = Patterns[tok.Type].Match(tok.Text);
                    Group fileMatch = match.Groups["File"];
                    if (fileMatch.Success)
                        currentFile = fileMatch.Value.Replace("\\\\", "\\");
                    Group lineMatch = match.Groups["Line"];
                    if (lineMatch.Success)
                        currentline = int.Parse(lineMatch.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
            }
            while (SkipList.Contains(tok.Type));

            LookAheadToken = tok;
            return tok;
        }
    }

    #endregion

    #region Token

    public enum TokenType
    {

            //Non terminal tokens:
            _NONE_  = 0,
            _UNDETERMINED_= 1,

            //Non terminal tokens:
            Start   = 2,
            Function= 3,
            PrimaryExpression= 4,
            ParenthesizedExpression= 5,
            UnaryExpression= 6,
            PowerExpression= 7,
            MultiplicativeExpression= 8,
            AdditiveExpression= 9,
            ConcatEpression= 10,
            RelationalExpression= 11,
            EqualityExpression= 12,
            ConditionalAndExpression= 13,
            ConditionalOrExpression= 14,
            AssignmentExpression= 15,
            Expression= 16,
            Params  = 17,
            Literal = 18,
            IntegerLiteral= 19,
            RealLiteral= 20,
            StringLiteral= 21,
            Variable= 22,

            //Terminal tokens:
            BOOLEANLITERAL= 23,
            DECIMALINTEGERLITERAL= 24,
            REALLITERAL= 25,
            HEXINTEGERLITERAL= 26,
            STRINGLITERAL= 27,
            FUNCTION= 28,
            VARIABLE= 29,
            CONSTANT= 30,
            BRACEOPEN= 31,
            BRACECLOSE= 32,
            BRACKETOPEN= 33,
            BRACKETCLOSE= 34,
            SEMICOLON= 35,
            PLUSPLUS= 36,
            MINUSMINUS= 37,
            PIPEPIPE= 38,
            AMPAMP  = 39,
            AMP     = 40,
            POWER   = 41,
            PLUS    = 42,
            MINUS   = 43,
            EQUAL   = 44,
            ASSIGN  = 45,
            NOTEQUAL= 46,
            NOT     = 47,
            ASTERIKS= 48,
            SLASH   = 49,
            PERCENT = 50,
            QUESTIONMARK= 51,
            COMMA   = 52,
            LESSEQUAL= 53,
            GREATEREQUAL= 54,
            LESSTHAN= 55,
            GREATERTHAN= 56,
            COLON   = 57,
            EOF     = 58,
            WHITESPACE= 59
    }

    public class Token
    {
        private string file;
        private int line;
        private int column;
        private int startpos;
        private int endpos;
        private string text;
        private object value;

        // contains all prior skipped symbols
        private List<Token> skipped;

        public string File { 
            get { return file; } 
            set { file = value; }
        }

        public int Line { 
            get { return line; } 
            set { line = value; }
        }

        public int Column {
            get { return column; } 
            set { column = value; }
        }

        public int StartPos { 
            get { return startpos;} 
            set { startpos = value; }
        }

        public int Length { 
            get { return endpos - startpos;} 
        }

        public int EndPos { 
            get { return endpos;} 
            set { endpos = value; }
        }

        public string Text { 
            get { return text;} 
            set { text = value; }
        }

        public List<Token> Skipped { 
            get { return skipped;} 
            set { skipped = value; }
        }
        public object Value { 
            get { return value;} 
            set { this.value = value; }
        }

        [XmlAttribute]
        public TokenType Type;

        public Token()
            : this(0, 0)
        {
        }

        public Token(int start, int end)
        {
            Type = TokenType._UNDETERMINED_;
            startpos = start;
            endpos = end;
            Text = ""; // must initialize with empty string, may cause null reference exceptions otherwise
            Value = null;
        }

        public void UpdateRange(Token token)
        {
            if (token.StartPos < startpos) startpos = token.StartPos;
            if (token.EndPos > endpos) endpos = token.EndPos;
        }

        public override string ToString()
        {
            if (Text != null)
                return Type.ToString() + " '" + Text + "'";
            else
                return Type.ToString();
        }


    }

    #endregion
}
