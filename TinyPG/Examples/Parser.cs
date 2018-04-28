// Automatically generated from source file: C:\Users\Ping\Documents\Visual Studio 2015\Projects\TinyPG_gitmine\TinyPG\Examples\tiny_language2.tpg
// By TinyPG v1.3 available at http://github.com/SickheadGames/TinyPG

using System;
using System.Collections.Generic;

// Disable unused variable warnings which
// can happen during the parser generation.
#pragma warning disable 168

namespace TinyExe
{
    #region Parser

    public partial class Parser 
    {
        private Scanner scanner;
        private ParseTree tree;
        
        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

         public ParseTree Parse(string input)
        {
            return Parse(input, "", new ParseTree());
        }

        public ParseTree Parse(string input, string fileName)
        {
            return Parse(input, fileName, new ParseTree());
        }

        public ParseTree Parse(string input, string fileName, ParseTree tree)
        {
            scanner.Init(input, fileName);

            this.tree = tree;
            ParseStart(tree);
            tree.Skipped = scanner.Skipped;

            return tree;
        }

        private void ParseStart(ParseNode parent) // NonTerminalSymbol: Start
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Start), "Start");
            parent.Nodes.Add(node);


             // Concat Rule
            tok = scanner.LookAhead(TokenType.FUNCTION, TokenType.VARIABLE, TokenType.BOOLEANLITERAL, TokenType.DECIMALINTEGERLITERAL, TokenType.HEXINTEGERLITERAL, TokenType.REALLITERAL, TokenType.STRINGLITERAL, TokenType.BRACKETOPEN, TokenType.PLUS, TokenType.MINUS, TokenType.NOT, TokenType.ASSIGN); // ZeroOrMore Rule
            while (tok.Type == TokenType.FUNCTION
                || tok.Type == TokenType.VARIABLE
                || tok.Type == TokenType.BOOLEANLITERAL
                || tok.Type == TokenType.DECIMALINTEGERLITERAL
                || tok.Type == TokenType.HEXINTEGERLITERAL
                || tok.Type == TokenType.REALLITERAL
                || tok.Type == TokenType.STRINGLITERAL
                || tok.Type == TokenType.BRACKETOPEN
                || tok.Type == TokenType.PLUS
                || tok.Type == TokenType.MINUS
                || tok.Type == TokenType.NOT
                || tok.Type == TokenType.ASSIGN)
            {
                ParseExpression(node); // NonTerminal Rule: Expression
            tok = scanner.LookAhead(TokenType.EOF, TokenType.FUNCTION, TokenType.VARIABLE, TokenType.BOOLEANLITERAL, TokenType.DECIMALINTEGERLITERAL, TokenType.HEXINTEGERLITERAL, TokenType.REALLITERAL, TokenType.STRINGLITERAL, TokenType.BRACKETOPEN, TokenType.PLUS, TokenType.MINUS, TokenType.NOT, TokenType.ASSIGN); // ZeroOrMore Rule
            }

             // Concat Rule
            tok = scanner.Scan(TokenType.EOF); // Terminal Rule: EOF
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.EOF) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EOF.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: Start

        private void ParseFunction(ParseNode parent) // NonTerminalSymbol: Function
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Function), "Function");
            parent.Nodes.Add(node);


             // Concat Rule
            tok = scanner.Scan(TokenType.FUNCTION); // Terminal Rule: FUNCTION
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.FUNCTION) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.FUNCTION.ToString(), 0x1001, tok));
                return;
            }

             // Concat Rule
            tok = scanner.Scan(TokenType.BRACKETOPEN); // Terminal Rule: BRACKETOPEN
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.BRACKETOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETOPEN.ToString(), 0x1001, tok));
                return;
            }

             // Concat Rule
            tok = scanner.LookAhead(TokenType.FUNCTION, TokenType.VARIABLE, TokenType.BOOLEANLITERAL, TokenType.DECIMALINTEGERLITERAL, TokenType.HEXINTEGERLITERAL, TokenType.REALLITERAL, TokenType.STRINGLITERAL, TokenType.BRACKETOPEN, TokenType.PLUS, TokenType.MINUS, TokenType.NOT, TokenType.ASSIGN, TokenType.SEMICOLON); // Option Rule
            if (tok.Type == TokenType.FUNCTION
                || tok.Type == TokenType.VARIABLE
                || tok.Type == TokenType.BOOLEANLITERAL
                || tok.Type == TokenType.DECIMALINTEGERLITERAL
                || tok.Type == TokenType.HEXINTEGERLITERAL
                || tok.Type == TokenType.REALLITERAL
                || tok.Type == TokenType.STRINGLITERAL
                || tok.Type == TokenType.BRACKETOPEN
                || tok.Type == TokenType.PLUS
                || tok.Type == TokenType.MINUS
                || tok.Type == TokenType.NOT
                || tok.Type == TokenType.ASSIGN
                || tok.Type == TokenType.SEMICOLON)
            {
                ParseParams(node); // NonTerminal Rule: Params
            }

             // Concat Rule
            tok = scanner.Scan(TokenType.BRACKETCLOSE); // Terminal Rule: BRACKETCLOSE
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.BRACKETCLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETCLOSE.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: Function

        private void ParsePrimaryExpression(ParseNode parent) // NonTerminalSymbol: PrimaryExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.PrimaryExpression), "PrimaryExpression");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.FUNCTION, TokenType.VARIABLE, TokenType.BOOLEANLITERAL, TokenType.DECIMALINTEGERLITERAL, TokenType.HEXINTEGERLITERAL, TokenType.REALLITERAL, TokenType.STRINGLITERAL, TokenType.BRACKETOPEN); // Choice Rule
            switch (tok.Type)
            { // Choice Rule
                case TokenType.FUNCTION:
                    ParseFunction(node); // NonTerminal Rule: Function
                    break;
                case TokenType.VARIABLE:
                    ParseVariable(node); // NonTerminal Rule: Variable
                    break;
                case TokenType.BOOLEANLITERAL:
                case TokenType.DECIMALINTEGERLITERAL:
                case TokenType.HEXINTEGERLITERAL:
                case TokenType.REALLITERAL:
                case TokenType.STRINGLITERAL:
                    ParseLiteral(node); // NonTerminal Rule: Literal
                    break;
                case TokenType.BRACKETOPEN:
                    ParseParenthesizedExpression(node); // NonTerminal Rule: ParenthesizedExpression
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected FUNCTION, VARIABLE, BOOLEANLITERAL, DECIMALINTEGERLITERAL, HEXINTEGERLITERAL, REALLITERAL, STRINGLITERAL, or BRACKETOPEN.", 0x0002, tok));
                    break;
            } // Choice Rule

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: PrimaryExpression

        private void ParseParenthesizedExpression(ParseNode parent) // NonTerminalSymbol: ParenthesizedExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ParenthesizedExpression), "ParenthesizedExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            tok = scanner.Scan(TokenType.BRACKETOPEN); // Terminal Rule: BRACKETOPEN
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.BRACKETOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETOPEN.ToString(), 0x1001, tok));
                return;
            }

             // Concat Rule
            ParseExpression(node); // NonTerminal Rule: Expression

             // Concat Rule
            tok = scanner.Scan(TokenType.BRACKETCLOSE); // Terminal Rule: BRACKETCLOSE
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.BRACKETCLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETCLOSE.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: ParenthesizedExpression

        private void ParseUnaryExpression(ParseNode parent) // NonTerminalSymbol: UnaryExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.UnaryExpression), "UnaryExpression");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.FUNCTION, TokenType.VARIABLE, TokenType.BOOLEANLITERAL, TokenType.DECIMALINTEGERLITERAL, TokenType.HEXINTEGERLITERAL, TokenType.REALLITERAL, TokenType.STRINGLITERAL, TokenType.BRACKETOPEN, TokenType.PLUS, TokenType.MINUS, TokenType.NOT); // Choice Rule
            switch (tok.Type)
            { // Choice Rule
                case TokenType.FUNCTION:
                case TokenType.VARIABLE:
                case TokenType.BOOLEANLITERAL:
                case TokenType.DECIMALINTEGERLITERAL:
                case TokenType.HEXINTEGERLITERAL:
                case TokenType.REALLITERAL:
                case TokenType.STRINGLITERAL:
                case TokenType.BRACKETOPEN:
                    ParsePrimaryExpression(node); // NonTerminal Rule: PrimaryExpression
                    break;
                case TokenType.PLUS:

                     // Concat Rule
                    tok = scanner.Scan(TokenType.PLUS); // Terminal Rule: PLUS
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.PLUS) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PLUS.ToString(), 0x1001, tok));
                        return;
                    }

                     // Concat Rule
                    ParseUnaryExpression(node); // NonTerminal Rule: UnaryExpression
                    break;
                case TokenType.MINUS:

                     // Concat Rule
                    tok = scanner.Scan(TokenType.MINUS); // Terminal Rule: MINUS
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.MINUS) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MINUS.ToString(), 0x1001, tok));
                        return;
                    }

                     // Concat Rule
                    ParseUnaryExpression(node); // NonTerminal Rule: UnaryExpression
                    break;
                case TokenType.NOT:

                     // Concat Rule
                    tok = scanner.Scan(TokenType.NOT); // Terminal Rule: NOT
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.NOT) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NOT.ToString(), 0x1001, tok));
                        return;
                    }

                     // Concat Rule
                    ParseUnaryExpression(node); // NonTerminal Rule: UnaryExpression
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected FUNCTION, VARIABLE, BOOLEANLITERAL, DECIMALINTEGERLITERAL, HEXINTEGERLITERAL, REALLITERAL, STRINGLITERAL, BRACKETOPEN, PLUS, MINUS, or NOT.", 0x0002, tok));
                    break;
            } // Choice Rule

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: UnaryExpression

        private void ParsePowerExpression(ParseNode parent) // NonTerminalSymbol: PowerExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.PowerExpression), "PowerExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseUnaryExpression(node); // NonTerminal Rule: UnaryExpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.POWER); // ZeroOrMore Rule
            while (tok.Type == TokenType.POWER)
            {

                 // Concat Rule
                tok = scanner.Scan(TokenType.POWER); // Terminal Rule: POWER
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.POWER) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.POWER.ToString(), 0x1001, tok));
                    return;
                }

                 // Concat Rule
                ParseUnaryExpression(node); // NonTerminal Rule: UnaryExpression
            tok = scanner.LookAhead(TokenType.POWER); // ZeroOrMore Rule
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: PowerExpression

        private void ParseMultiplicativeExpression(ParseNode parent) // NonTerminalSymbol: MultiplicativeExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.MultiplicativeExpression), "MultiplicativeExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParsePowerExpression(node); // NonTerminal Rule: PowerExpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.ASTERIKS, TokenType.SLASH, TokenType.PERCENT); // ZeroOrMore Rule
            while (tok.Type == TokenType.ASTERIKS
                || tok.Type == TokenType.SLASH
                || tok.Type == TokenType.PERCENT)
            {

                 // Concat Rule
                tok = scanner.LookAhead(TokenType.ASTERIKS, TokenType.SLASH, TokenType.PERCENT); // Choice Rule
                switch (tok.Type)
                { // Choice Rule
                    case TokenType.ASTERIKS:
                        tok = scanner.Scan(TokenType.ASTERIKS); // Terminal Rule: ASTERIKS
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.ASTERIKS) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.ASTERIKS.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    case TokenType.SLASH:
                        tok = scanner.Scan(TokenType.SLASH); // Terminal Rule: SLASH
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.SLASH) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SLASH.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    case TokenType.PERCENT:
                        tok = scanner.Scan(TokenType.PERCENT); // Terminal Rule: PERCENT
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.PERCENT) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PERCENT.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected ASTERIKS, SLASH, or PERCENT.", 0x0002, tok));
                        break;
                } // Choice Rule

                 // Concat Rule
                ParsePowerExpression(node); // NonTerminal Rule: PowerExpression
            tok = scanner.LookAhead(TokenType.ASTERIKS, TokenType.SLASH, TokenType.PERCENT); // ZeroOrMore Rule
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: MultiplicativeExpression

        private void ParseAdditiveExpression(ParseNode parent) // NonTerminalSymbol: AdditiveExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.AdditiveExpression), "AdditiveExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseMultiplicativeExpression(node); // NonTerminal Rule: MultiplicativeExpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS); // ZeroOrMore Rule
            while (tok.Type == TokenType.PLUS
                || tok.Type == TokenType.MINUS)
            {

                 // Concat Rule
                tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS); // Choice Rule
                switch (tok.Type)
                { // Choice Rule
                    case TokenType.PLUS:
                        tok = scanner.Scan(TokenType.PLUS); // Terminal Rule: PLUS
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.PLUS) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PLUS.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    case TokenType.MINUS:
                        tok = scanner.Scan(TokenType.MINUS); // Terminal Rule: MINUS
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.MINUS) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MINUS.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected PLUS or MINUS.", 0x0002, tok));
                        break;
                } // Choice Rule

                 // Concat Rule
                ParseMultiplicativeExpression(node); // NonTerminal Rule: MultiplicativeExpression
            tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS); // ZeroOrMore Rule
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: AdditiveExpression

        private void ParseConcatEpression(ParseNode parent) // NonTerminalSymbol: ConcatEpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ConcatEpression), "ConcatEpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseAdditiveExpression(node); // NonTerminal Rule: AdditiveExpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.AMP); // ZeroOrMore Rule
            while (tok.Type == TokenType.AMP)
            {

                 // Concat Rule
                tok = scanner.Scan(TokenType.AMP); // Terminal Rule: AMP
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.AMP) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.AMP.ToString(), 0x1001, tok));
                    return;
                }

                 // Concat Rule
                ParseAdditiveExpression(node); // NonTerminal Rule: AdditiveExpression
            tok = scanner.LookAhead(TokenType.AMP); // ZeroOrMore Rule
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: ConcatEpression

        private void ParseRelationalExpression(ParseNode parent) // NonTerminalSymbol: RelationalExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.RelationalExpression), "RelationalExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseConcatEpression(node); // NonTerminal Rule: ConcatEpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.LESSTHAN, TokenType.LESSEQUAL, TokenType.GREATERTHAN, TokenType.GREATEREQUAL); // Option Rule
            if (tok.Type == TokenType.LESSTHAN
                || tok.Type == TokenType.LESSEQUAL
                || tok.Type == TokenType.GREATERTHAN
                || tok.Type == TokenType.GREATEREQUAL)
            {

                 // Concat Rule
                tok = scanner.LookAhead(TokenType.LESSTHAN, TokenType.LESSEQUAL, TokenType.GREATERTHAN, TokenType.GREATEREQUAL); // Choice Rule
                switch (tok.Type)
                { // Choice Rule
                    case TokenType.LESSTHAN:
                        tok = scanner.Scan(TokenType.LESSTHAN); // Terminal Rule: LESSTHAN
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.LESSTHAN) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LESSTHAN.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    case TokenType.LESSEQUAL:
                        tok = scanner.Scan(TokenType.LESSEQUAL); // Terminal Rule: LESSEQUAL
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.LESSEQUAL) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.LESSEQUAL.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    case TokenType.GREATERTHAN:
                        tok = scanner.Scan(TokenType.GREATERTHAN); // Terminal Rule: GREATERTHAN
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.GREATERTHAN) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.GREATERTHAN.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    case TokenType.GREATEREQUAL:
                        tok = scanner.Scan(TokenType.GREATEREQUAL); // Terminal Rule: GREATEREQUAL
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.GREATEREQUAL) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.GREATEREQUAL.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected LESSTHAN, LESSEQUAL, GREATERTHAN, or GREATEREQUAL.", 0x0002, tok));
                        break;
                } // Choice Rule

                 // Concat Rule
                ParseConcatEpression(node); // NonTerminal Rule: ConcatEpression
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: RelationalExpression

        private void ParseEqualityExpression(ParseNode parent) // NonTerminalSymbol: EqualityExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.EqualityExpression), "EqualityExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseRelationalExpression(node); // NonTerminal Rule: RelationalExpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.EQUAL, TokenType.NOTEQUAL); // ZeroOrMore Rule
            while (tok.Type == TokenType.EQUAL
                || tok.Type == TokenType.NOTEQUAL)
            {

                 // Concat Rule
                tok = scanner.LookAhead(TokenType.EQUAL, TokenType.NOTEQUAL); // Choice Rule
                switch (tok.Type)
                { // Choice Rule
                    case TokenType.EQUAL:
                        tok = scanner.Scan(TokenType.EQUAL); // Terminal Rule: EQUAL
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.EQUAL) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EQUAL.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    case TokenType.NOTEQUAL:
                        tok = scanner.Scan(TokenType.NOTEQUAL); // Terminal Rule: NOTEQUAL
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.NOTEQUAL) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NOTEQUAL.ToString(), 0x1001, tok));
                            return;
                        }
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected EQUAL or NOTEQUAL.", 0x0002, tok));
                        break;
                } // Choice Rule

                 // Concat Rule
                ParseRelationalExpression(node); // NonTerminal Rule: RelationalExpression
            tok = scanner.LookAhead(TokenType.EQUAL, TokenType.NOTEQUAL); // ZeroOrMore Rule
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: EqualityExpression

        private void ParseConditionalAndExpression(ParseNode parent) // NonTerminalSymbol: ConditionalAndExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ConditionalAndExpression), "ConditionalAndExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseEqualityExpression(node); // NonTerminal Rule: EqualityExpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.AMPAMP); // ZeroOrMore Rule
            while (tok.Type == TokenType.AMPAMP)
            {

                 // Concat Rule
                tok = scanner.Scan(TokenType.AMPAMP); // Terminal Rule: AMPAMP
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.AMPAMP) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.AMPAMP.ToString(), 0x1001, tok));
                    return;
                }

                 // Concat Rule
                ParseEqualityExpression(node); // NonTerminal Rule: EqualityExpression
            tok = scanner.LookAhead(TokenType.AMPAMP); // ZeroOrMore Rule
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: ConditionalAndExpression

        private void ParseConditionalOrExpression(ParseNode parent) // NonTerminalSymbol: ConditionalOrExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ConditionalOrExpression), "ConditionalOrExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseConditionalAndExpression(node); // NonTerminal Rule: ConditionalAndExpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.PIPEPIPE); // ZeroOrMore Rule
            while (tok.Type == TokenType.PIPEPIPE)
            {

                 // Concat Rule
                tok = scanner.Scan(TokenType.PIPEPIPE); // Terminal Rule: PIPEPIPE
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.PIPEPIPE) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PIPEPIPE.ToString(), 0x1001, tok));
                    return;
                }

                 // Concat Rule
                ParseConditionalAndExpression(node); // NonTerminal Rule: ConditionalAndExpression
            tok = scanner.LookAhead(TokenType.PIPEPIPE); // ZeroOrMore Rule
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: ConditionalOrExpression

        private void ParseAssignmentExpression(ParseNode parent) // NonTerminalSymbol: AssignmentExpression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.AssignmentExpression), "AssignmentExpression");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseConditionalOrExpression(node); // NonTerminal Rule: ConditionalOrExpression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.QUESTIONMARK); // Option Rule
            if (tok.Type == TokenType.QUESTIONMARK)
            {

                 // Concat Rule
                tok = scanner.Scan(TokenType.QUESTIONMARK); // Terminal Rule: QUESTIONMARK
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.QUESTIONMARK) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.QUESTIONMARK.ToString(), 0x1001, tok));
                    return;
                }

                 // Concat Rule
                ParseAssignmentExpression(node); // NonTerminal Rule: AssignmentExpression

                 // Concat Rule
                tok = scanner.Scan(TokenType.COLON); // Terminal Rule: COLON
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.COLON) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COLON.ToString(), 0x1001, tok));
                    return;
                }

                 // Concat Rule
                ParseAssignmentExpression(node); // NonTerminal Rule: AssignmentExpression
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: AssignmentExpression

        private void ParseExpression(ParseNode parent) // NonTerminalSymbol: Expression
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Expression), "Expression");
            parent.Nodes.Add(node);


             // Concat Rule
            tok = scanner.LookAhead(TokenType.FUNCTION, TokenType.VARIABLE, TokenType.BOOLEANLITERAL, TokenType.DECIMALINTEGERLITERAL, TokenType.HEXINTEGERLITERAL, TokenType.REALLITERAL, TokenType.STRINGLITERAL, TokenType.BRACKETOPEN, TokenType.PLUS, TokenType.MINUS, TokenType.NOT); // Option Rule
            if (tok.Type == TokenType.FUNCTION
                || tok.Type == TokenType.VARIABLE
                || tok.Type == TokenType.BOOLEANLITERAL
                || tok.Type == TokenType.DECIMALINTEGERLITERAL
                || tok.Type == TokenType.HEXINTEGERLITERAL
                || tok.Type == TokenType.REALLITERAL
                || tok.Type == TokenType.STRINGLITERAL
                || tok.Type == TokenType.BRACKETOPEN
                || tok.Type == TokenType.PLUS
                || tok.Type == TokenType.MINUS
                || tok.Type == TokenType.NOT)
            {
                ParseAssignmentExpression(node); // NonTerminal Rule: AssignmentExpression
            }

             // Concat Rule
            tok = scanner.LookAhead(TokenType.ASSIGN); // Option Rule
            if (tok.Type == TokenType.ASSIGN)
            {

                 // Concat Rule
                tok = scanner.Scan(TokenType.ASSIGN); // Terminal Rule: ASSIGN
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.ASSIGN) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.ASSIGN.ToString(), 0x1001, tok));
                    return;
                }

                 // Concat Rule
                ParseAssignmentExpression(node); // NonTerminal Rule: AssignmentExpression
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: Expression

        private void ParseParams(ParseNode parent) // NonTerminalSymbol: Params
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Params), "Params");
            parent.Nodes.Add(node);


             // Concat Rule
            ParseExpression(node); // NonTerminal Rule: Expression

             // Concat Rule
            tok = scanner.LookAhead(TokenType.SEMICOLON); // ZeroOrMore Rule
            while (tok.Type == TokenType.SEMICOLON)
            {

                 // Concat Rule
                tok = scanner.Scan(TokenType.SEMICOLON); // Terminal Rule: SEMICOLON
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.SEMICOLON) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOLON.ToString(), 0x1001, tok));
                    return;
                }

                 // Concat Rule
                ParseExpression(node); // NonTerminal Rule: Expression
            tok = scanner.LookAhead(TokenType.SEMICOLON); // ZeroOrMore Rule
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: Params

        private void ParseLiteral(ParseNode parent) // NonTerminalSymbol: Literal
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Literal), "Literal");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.BOOLEANLITERAL, TokenType.DECIMALINTEGERLITERAL, TokenType.HEXINTEGERLITERAL, TokenType.REALLITERAL, TokenType.STRINGLITERAL); // Choice Rule
            switch (tok.Type)
            { // Choice Rule
                case TokenType.BOOLEANLITERAL:
                    tok = scanner.Scan(TokenType.BOOLEANLITERAL); // Terminal Rule: BOOLEANLITERAL
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.BOOLEANLITERAL) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BOOLEANLITERAL.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.DECIMALINTEGERLITERAL:
                case TokenType.HEXINTEGERLITERAL:
                    ParseIntegerLiteral(node); // NonTerminal Rule: IntegerLiteral
                    break;
                case TokenType.REALLITERAL:
                    ParseRealLiteral(node); // NonTerminal Rule: RealLiteral
                    break;
                case TokenType.STRINGLITERAL:
                    ParseStringLiteral(node); // NonTerminal Rule: StringLiteral
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected BOOLEANLITERAL, DECIMALINTEGERLITERAL, HEXINTEGERLITERAL, REALLITERAL, or STRINGLITERAL.", 0x0002, tok));
                    break;
            } // Choice Rule

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: Literal

        private void ParseIntegerLiteral(ParseNode parent) // NonTerminalSymbol: IntegerLiteral
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.IntegerLiteral), "IntegerLiteral");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.DECIMALINTEGERLITERAL, TokenType.HEXINTEGERLITERAL); // Choice Rule
            switch (tok.Type)
            { // Choice Rule
                case TokenType.DECIMALINTEGERLITERAL:
                    tok = scanner.Scan(TokenType.DECIMALINTEGERLITERAL); // Terminal Rule: DECIMALINTEGERLITERAL
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.DECIMALINTEGERLITERAL) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DECIMALINTEGERLITERAL.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.HEXINTEGERLITERAL:
                    tok = scanner.Scan(TokenType.HEXINTEGERLITERAL); // Terminal Rule: HEXINTEGERLITERAL
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.HEXINTEGERLITERAL) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.HEXINTEGERLITERAL.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected DECIMALINTEGERLITERAL or HEXINTEGERLITERAL.", 0x0002, tok));
                    break;
            } // Choice Rule

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: IntegerLiteral

        private void ParseRealLiteral(ParseNode parent) // NonTerminalSymbol: RealLiteral
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.RealLiteral), "RealLiteral");
            parent.Nodes.Add(node);

            tok = scanner.Scan(TokenType.REALLITERAL); // Terminal Rule: REALLITERAL
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.REALLITERAL) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.REALLITERAL.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: RealLiteral

        private void ParseStringLiteral(ParseNode parent) // NonTerminalSymbol: StringLiteral
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.StringLiteral), "StringLiteral");
            parent.Nodes.Add(node);

            tok = scanner.Scan(TokenType.STRINGLITERAL); // Terminal Rule: STRINGLITERAL
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.STRINGLITERAL) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRINGLITERAL.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: StringLiteral

        private void ParseVariable(ParseNode parent) // NonTerminalSymbol: Variable
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Variable), "Variable");
            parent.Nodes.Add(node);

            tok = scanner.Scan(TokenType.VARIABLE); // Terminal Rule: VARIABLE
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.VARIABLE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.VARIABLE.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        } // NonTerminalSymbol: Variable




    }

    #endregion Parser
}
