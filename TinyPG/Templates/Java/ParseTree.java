// Automatically generated from source file: <%SourceFilename%>
// By TinyPG v1.3 available at http://github.com/SickheadGames/TinyPG

package <%Namespace%>;
import java.util.ArrayList;
import java.util.List;


class ParseErrors extends <%ParseErrors%>
{
}

class ParseError<%ParseError%>
{
	private String file;
	private String message;
	private int code;
	private int line;
	private int col;
	private int pos;
	private int length;
	

	public String getFile() { return file; }
	public int getCode() { return code; }
	public int getLine() { return line; }
	public int getColumn() { return col; }
	public int getPosition() { return pos; }
	public int getLength() { return length; }
	public String getMessage() { return message; }

	// just for the sake of serialization
	public ParseError()
	{
	}

	public ParseError(String message, int code, ParseNode node)
	{
		this(message, code, node.Token);
	}

	public ParseError(String message, int code, Token token)
	{
		this(message, code, token.getFile(), token.getLine(), token.getColumn(), token.getStartPos(), token.getLength());
	}

	public ParseError(String message, int code)
	{
		this(message, code, "", 0, 0, 0, 0);
	}

	public ParseError(String message, int code, String file, int line, int col, int pos, int length)
	{
		this.file = file;
		this.message = message;
		this.code = code;
		this.line = line;
		this.col = col;
		this.pos = pos;
		this.length = length;
	}
}

// rootlevel of the node tree
public class ParseTree extends ParseNode<%IParseTree%>
{
	public ParseErrors Errors;

	public ArrayList<Token> Skipped;

	public ParseTree()
	{
		super(new Token(), "ParseTree");
		Token.Type = TokenType.Start;
		Token.setText("Root");
		Errors = new ParseErrors();
	}

	public String PrintTree()
	{
		StringBuilder sb = new StringBuilder();
		int indent = 0;
		PrintNode(sb, this, indent);
		return sb.toString();
	}

	private void PrintNode(StringBuilder sb, ParseNode node, int indent)
	{
		
		for(int i=0;i<indent;i++) {
			sb.append(' ');
		}

		
		sb.append(node.getText() + "\n");

		for (ParseNode n : node.getNodes())
			PrintNode(sb, n, indent + 2);
	}
	
	/// <summary>
	/// this is the entry point for executing and evaluating the parse tree.
	/// </summary>
	/// <param name="paramlist">additional optional input parameters</param>
	/// <returns>the output of the evaluation function</returns>
	public Object Eval(Object... paramlist)
	{
		return getNodes().get(0).Eval(this, paramlist);
	}
}

class ParseNode<%IParseNode%>
{
	protected String text;
	protected ArrayList<ParseNode> nodes;
	<%ITokenGet%>
	public ArrayList<ParseNode> getNodes() { return nodes;}
	<%INodesGet%>
	//[XmlIgnore] // avoid circular references when serializing
	public ParseNode Parent;
	public Token Token; // the token/rule

	//[XmlIgnore] // skip redundant text (is part of Token)
	public String getText() { // text to display in parse tree 
		return text;
	}
	
	public void setText(String value) { text = value; }

	public ParseNode CreateNode(Token token, String text)
	{
		ParseNode node = new ParseNode(token, text);
		node.Parent = this;
		return node;
	}

	protected ParseNode(Token token, String text)
	{
		this.Token = token;
		this.text = text;
		this.nodes = new ArrayList<ParseNode>();
	}

	protected Object GetValue(ParseTree tree, TokenType type, int index)
	{
		return GetValue(tree, type, new int[]{ index });
	}

	protected Object GetValue(ParseTree tree, TokenType type, int[] index)
	{
		Object o = null;
		if (index[0] < 0) return o;

		// left to right
		for (ParseNode node : nodes)
		{
			if (node.Token.Type == type)
			{
				index[0]--;
				if (index[0] < 0)
				{
					o = node.Eval(tree);
					break;
				}
			}
		}
		return o;
	}

	/// <summary>
	/// this implements the evaluation functionality, cannot be used directly
	/// </summary>
	/// <param name="tree">the parsetree itself</param>
	/// <param name="paramlist">optional input parameters</param>
	/// <returns>a partial result of the evaluation</returns>
	public Object Eval(ParseTree tree, Object... paramlist)
	{
		Object Value = null;

		switch (Token.Type)
		{
<%EvalSymbols%>
			default:
				Value = Token.getText();
				break;
		}
		return Value;
	}

<%VirtualEvalMethods%>


<%ParseTreeCustomCode%>
}
    
