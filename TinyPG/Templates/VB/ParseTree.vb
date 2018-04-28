'Automatically generated from source file: <%SourceFilename%>
'By TinyPG v1.3 available at http://github.com/SickheadGames/TinyPG

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml.Serialization
<%Imports%>

Namespace <%Namespace%>
#Region "ParseTree"
    <Serializable()>
    Public Class ParseErrors
        Inherits <%ParseErrors%>

        Public Sub New()

        End Sub
    End Class

    <Serializable()>
    Public Class ParseError <%ParseError%>
        Private m_message As String
        Private m_code As Integer
        Private m_line As Integer
        Private m_col As Integer
        Private m_pos As Integer
        Private m_length As Integer

        Public ReadOnly Property Code() As Integer<%ImplementsIParseErrorCode%>
            Get
                Return m_code
            End Get
        End Property

        Public ReadOnly Property Line() As Integer<%ImplementsIParseErrorLine%>
            Get
                Return m_line
            End Get
        End Property

        Public ReadOnly Property Column() As Integer<%ImplementsIParseErrorColumn%>
            Get
                Return m_col
            End Get
        End Property

        Public ReadOnly Property Position() As Integer<%ImplementsIParseErrorPosition%>
            Get
                Return m_pos
            End Get
        End Property

        Public ReadOnly Property Length() As Integer<%ImplementsIParseErrorLength%>
            Get
                Return m_length
            End Get
        End Property

        Public ReadOnly Property Message() As String<%ImplementsIParseErrorMessage%>
            Get
                Return m_message
            End Get
        End Property

        Public Sub New(ByVal message As String, ByVal code As Integer, ByVal tok As Token)
            Me.New(message, code, 0, tok.StartPos, tok.StartPos, tok.Length)
        End Sub

        Public Sub New(ByVal message As String, ByVal code As Integer, ByVal line As Integer, ByVal col As Integer, ByVal pos As Integer, ByVal length As Integer)
            m_message = message
            m_code = code
            m_line = line
            m_col = col
            m_pos = pos
            m_length = length
        End Sub
    End Class

    ' rootlevel of the node tree
    <Serializable()>
    Partial Public Class ParseTree
        Inherits ParseNode<%IParseTree%>

        Public Errors As ParseErrors

        Public Skipped As List(Of Token)

        Public Sub New()
            MyBase.New(New Token(), "ParseTree")
            Token.Type = TokenType.Start
            Token.Text = "Root"
            Skipped = New List(Of Token)()
            Errors = New ParseErrors()
        End Sub

        Public Function PrintTree() As String<%ImplementsIParseTreePrintTree%>
    Dim sb As New StringBuilder()
            Dim indent As Integer = 0
            PrintNode(sb, Me, indent)
            Return sb.ToString()
        End Function

        Private Sub PrintNode(ByVal sb As StringBuilder, ByVal node As ParseNode, ByVal indent As Integer)

            Dim space As String = "".PadLeft(indent, " "c)

            sb.Append(space)
            sb.AppendLine(node.Text)

            For Each n As ParseNode In node.Nodes
                PrintNode(sb, n, indent + 2)
            Next
        End Sub

        ''' <summary>
        ''' this is the entry point for executing and evaluating the parse tree.
        ''' </summary>
        ''' <param name="paramlist">additional optional input parameters</param>
        ''' <returns>the output of the evaluation function</returns>
        Public Overloads Function Eval(ByVal ParamArray paramlist As Object()) As Object<%ImplementsIParseTreeEval%>
        Return Nodes(0).Eval(Me, paramlist)
        End Function
    End Class
#End Region

#Region "ParseNode"
    <Serializable()>
    <XmlInclude(GetType(ParseTree))>
    Partial Public Class ParseNode <%IParseNode%>
        Protected m_text As String
        Protected m_nodes As List(Of ParseNode)
        <%ITokenGet%>

        Public ReadOnly Property Nodes() As List(Of ParseNode)
            Get
                Return m_nodes
            End Get
        End Property

        <%INodesGet%>
        <XMLIgnore()>
        Public Parent As ParseNode
        Public Token As Token
        ' the token/rule
        <XmlIgnore()>
        Public Property Text() As String<%ImplementsIParseNodeText%>
            ' text to display in parse tree 
            Get
                Return m_text
            End Get
            Set(ByVal value As String)
                m_text = value
            End Set
        End Property

        Public Overridable Function CreateNode(ByVal token As Token, ByVal text As String) As ParseNode
            Dim node As New ParseNode(token, text)
            node.Parent = Me
            Return node
        End Function

        Protected Sub New(ByVal token As Token, ByVal text As String)
            Me.Token = token
            m_text = text
            m_nodes = New List(Of ParseNode)()
        End Sub

        Protected Function GetValue(ByVal tree As ParseTree, ByVal type As TokenType, ByVal index As Integer) As Object
            Return GetValueByRef(tree, type, index)
        End Function

        Protected Function GetValueByRef(ByVal tree As ParseTree, ByVal type As TokenType, ByRef index As Integer) As Object
            Dim o As Object = Nothing
            If index < 0 Then
                Return o
            End If

            ' left to right
            For Each node As ParseNode In Nodes
                If node.Token.Type = type Then
                    System.Math.Max(System.Threading.Interlocked.Decrement(index), index + 1)
                    If index < 0 Then
                        o = node.Eval(tree)
                        Exit For
                    End If
                End If
            Next
            Return o
        End Function

        ''' <summary>
        ''' this implements the evaluation functionality, cannot be used directly
        ''' </summary>
        ''' <param name="tree">the parsetree itself</param>
        ''' <param name="paramlist">optional input parameters</param>
        ''' <returns>a partial result of the evaluation</returns>
        Friend Function Eval(ByVal tree As ParseTree, ByVal ParamArray paramlist As Object()) As Object
            Dim Value As Object = Nothing

            Select Case Token.Type
<%EvalSymbols%>
                Case Else
                    Value = Token.Text
                    Exit Select
            End Select
            Return Value
        End Function

        <%VirtualEvalMethods%>

<%ParseTreeCustomCode%>
    End Class
#End Region
End Namespace

