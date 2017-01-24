'Automatically generated from source file: C:\Users\Ping\Documents\Visual Studio 2015\Projects\TinyPG_gitmine\TinyPG\Examples\simple expression2_vb.tpg
'By TinyPG v1.3 available at http://github.com/SickheadGames/TinyPG

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml.Serialization


Namespace TinyPG
#Region "ParseTree"
    <Serializable()> _
    Public Class ParseErrors
        Inherits List(Of ParseError)

        Public Sub New()

        End Sub
    End Class

    <Serializable()> _
    Public Class ParseError 
        Private m_message As String
        Private m_code As Integer
        Private m_line As Integer
        Private m_col As Integer
        Private m_pos As Integer
        Private m_length As Integer

        Public ReadOnly Property Code() As Integer
            Get
                Return m_code
            End Get
        End Property

        Public ReadOnly Property Line() As Integer
            Get
                Return m_line
            End Get
        End Property

        Public ReadOnly Property Column() As Integer
            Get
                Return m_col
            End Get
        End Property

        Public ReadOnly Property Position() As Integer
            Get
                Return m_pos
            End Get
        End Property

        Public ReadOnly Property Length() As Integer
            Get
                Return m_length
            End Get
        End Property

        Public ReadOnly Property Message() As String
            Get
                Return m_message
            End Get
        End Property

        Public Sub New(ByVal message As String, ByVal code As Integer, ByVal node As ParseNode)
            Me.New(message, code, 0, node.Token.StartPos, node.Token.StartPos, node.Token.Length)
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
    <Serializable()> _
    Partial Public Class ParseTree
        Inherits ParseNode

        Public Errors As ParseErrors

        Public Skipped As List(Of Token)

        Public Sub New()
            MyBase.New(New Token(), "ParseTree")
            Token.Type = TokenType.Start
            Token.Text = "Root"
            Skipped = New List(Of Token)()
            Errors = New ParseErrors()
        End Sub

        Public Function PrintTree() As String
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
    Public Overloads Function Eval(ByVal ParamArray paramlist As Object()) As Object
        Return Nodes(0).Eval(Me, paramlist)
    End Function
    End Class
#End Region

#Region "ParseNode"
    <Serializable()> _
    <XmlInclude(GetType(ParseTree))> _
    Partial Public Class ParseNode 
        Protected m_text As String
        Protected m_nodes As List(Of ParseNode)
        

        Public ReadOnly Property Nodes() As List(Of ParseNode)
            Get
                Return m_nodes
            End Get
        End Property

        
        <XMLIgnore()> _
        Public Parent As ParseNode
        Public Token As Token
        ' the token/rule
        <XmlIgnore()> _
        Public Property Text() As String
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
            For Each node As ParseNode In nodes
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
                Case TokenType.Start
                    Value = EvalStart(tree, paramlist)
                    Exit Select
                Case TokenType.AddExpr
                    Value = EvalAddExpr(tree, paramlist)
                    Exit Select
                Case TokenType.MultExpr
                    Value = EvalMultExpr(tree, paramlist)
                    Exit Select
                Case TokenType.Atom
                    Value = EvalAtom(tree, paramlist)
                    Exit Select

                Case Else
                    Value = Token.Text
                    Exit Select
            End Select
            Return Value
        End Function

        Protected Overridable Function EvalStart(ByVal tree As ParseTree, ByVal ParamArray paramlist As Object()) As Object
            Return Me.GetValue(tree, TokenType.AddExpr, 0)
        End Function

        Protected Overridable Function EvalAddExpr(ByVal tree As ParseTree, ByVal ParamArray paramlist As Object()) As Object
            Dim Value As Integer = Convert.ToInt32(Me.GetValue(tree, TokenType.MultExpr, 0))
        	Dim i As Integer = 1
        	While Me.GetValue(tree, TokenType.MultExpr, i) IsNot Nothing
        		Dim sign As String = Me.GetValue(tree, TokenType.PLUSMINUS, i-1).ToString()
        		If sign = "+" Then
        			Value += Convert.ToInt32(Me.GetValue(tree, TokenType.MultExpr, i))
        		Else 
        			Value -= Convert.ToInt32(Me.GetValue(tree, TokenType.MultExpr, i))
        		End If
        		i=i+1
        	End While
        	Return Value
        End Function

        Protected Overridable Function EvalMultExpr(ByVal tree As ParseTree, ByVal ParamArray paramlist As Object()) As Object
            Dim Value As Integer = Convert.ToInt32(Me.GetValue(tree, TokenType.Atom, 0))
        	Dim i As Integer = 1
        	While Me.GetValue(tree, TokenType.Atom, i) IsNot Nothing
        		Dim sign As String = Me.GetValue(tree, TokenType.MULTDIV, i-1).ToString()
        		If sign = "*" Then
        			Value *= Convert.ToInt32(Me.GetValue(tree, TokenType.Atom, i))
        		Else 
        			Value /= Convert.ToInt32(Me.GetValue(tree, TokenType.Atom, i))
        		End If
        		i=i+1
        	End While
        
        	Return Value
        End Function

        Protected Overridable Function EvalAtom(ByVal tree As ParseTree, ByVal ParamArray paramlist As Object()) As Object
            If Me.GetValue(tree, TokenType.NUMBER, 0) IsNot Nothing Then
        		Return Me.GetValue(tree, TokenType.NUMBER, 0)
        	ElseIf Me.GetValue(tree, TokenType.ID, 0) != null
        		Return GetVarValue(Me.GetValue(tree, TokenType.ID, 0).ToString())
        	Else 
        		Return Me.GetValue(tree, TokenType.AddExpr, 0)
        	End If
        End Function




		Protected context as System.Collections.Generic.Dictionary<string,int>
		Public Property Context as System.Collections.Generic.Dictionary<string,int>
			Get
				if(context == null && this.Parent != null) {
					return Parent.Context;
				}
				return null;
			End Get
			Set
				context = value;
			End Set
		End Property

		Public Sub Function GetVarValue(id as String)
			return Context == null?0:Context[id];
		End Sub

    End Class
#End Region
End Namespace

