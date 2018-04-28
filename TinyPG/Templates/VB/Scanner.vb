'Automatically generated from source file: <%SourceFilename%>
'By TinyPG v1.3 available at http://github.com/SickheadGames/TinyPG

Imports System
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
<%Imports%>

Namespace <%Namespace%>
#Region "Scanner"

    Partial Public Class Scanner
        Public Input As String
        Public StartPos As Integer = 0
        Public EndPos As Integer = 0
        Public CurrentLine As Integer
        Public CurrentColumn As Integer
        Public CurrentPosition As Integer
        Public Skipped As List(Of Token) ' tokens that were skipped
        Public Patterns As Dictionary(Of TokenType, Regex)

        Private LookAheadToken As Token
        Private Tokens As List(Of TokenType)
        Private SkipList As List(Of TokenType) ' tokens to be skipped

        Public Sub New()
            Dim regex As Regex
            Patterns = New Dictionary(Of TokenType, Regex)()
            Tokens = New List(Of TokenType)()
            LookAheadToken = Nothing
            Skipped = New List(Of Token)()

            SkipList = New List(Of TokenType)()
<%SkipList%>
<%RegExps%>
        End Sub

        Public Sub Init(ByVal input As String)
            Me.Input = input
            StartPos = 0
            EndPos = 0
            CurrentLine = 0
            CurrentColumn = 0
            CurrentPosition = 0
            Skipped = New List(Of Token)()
            LookAheadToken = Nothing
        End Sub

        Public Function GetToken(ByVal type As TokenType) As Token
            Dim t As New Token(Me.StartPos, Me.EndPos)
            t.Type = type
            Return t
        End Function

        ''' <summary>
        ''' executes a lookahead of the next token
        ''' and will advance the scan on the input string
        ''' </summary>
        ''' <returns></returns>
        Public Function Scan(ByVal ParamArray expectedtokens As TokenType()) As Token
            Dim tok As Token = LookAhead(expectedtokens)
            ' temporarely retrieve the lookahead
            LookAheadToken = Nothing
            ' reset lookahead token, so scanning will continue
            StartPos = tok.EndPos
            EndPos = tok.EndPos
            ' set the tokenizer to the new scan position
            Return tok
        End Function

        ''' <summary>
        ''' returns token with longest best match
        ''' </summary>
        ''' <returns></returns>
        Public Function LookAhead(ByVal ParamArray expectedtokens As TokenType()) As Token
            Dim i As Integer
            Dim start As Integer = StartPos
            Dim tok As Token = Nothing
            Dim scantokens As List(Of TokenType)

            ' this prevents double scanning and matching
            ' increased performance
            If LookAheadToken IsNot Nothing AndAlso LookAheadToken.Type <> TokenType._UNDETERMINED_ AndAlso LookAheadToken.Type <> TokenType._NONE_ Then
                Return LookAheadToken
            End If

            If expectedtokens.Length = 0 Then
                scantokens = Tokens
            Else
                scantokens = New List(Of TokenType)(expectedtokens)
                scantokens.AddRange(SkipList)
            End If

            Do
                Dim len As Integer = -1
                Dim index As TokenType = Integer.MaxValue
                Dim m_input As String = Input.Substring(start)

                tok = New Token(start, EndPos)


                For i = 0 To scantokens.Count - 1
                    Dim r As Regex = Patterns(scantokens(i))
                    Dim m As Match = r.Match(m_input)
                    If m.Success AndAlso m.Index = 0 AndAlso ((m.Length > len) OrElse (scantokens(i) < index AndAlso m.Length = len)) Then
                        len = m.Length
                        index = scantokens(i)
                    End If
                Next i

                If index >= 0 AndAlso len >= 0 Then
                    tok.EndPos = start + len
                    tok.Text = Input.Substring(tok.StartPos, len)
                    tok.Type = index
                Else
                    If tok.StartPos < tok.EndPos - 1 Then
                        tok.Text = Input.Substring(tok.StartPos, 1)
                    End If
                End If

                If SkipList.Contains(tok.Type) Then
                    start = tok.EndPos
                    Skipped.Add(tok)
                Else
                    tok.Skipped = Skipped
                    Skipped = New List(Of Token)
                End If
            Loop While SkipList.Contains(tok.Type)

            LookAheadToken = tok
            Return tok
        End Function
    End Class
#End Region

#Region "Token"

    Public Enum TokenType
<%TokenType%>
    End Enum

    Public Class Token <%IToken%>
        Private m_startPos As Integer
        Private m_endPos As Integer
        Private m_text As String
        Private m_value As Object

        ' contains all prior skipped symbols
        Private m_skipped As List(Of Token)


        Public Property StartPos() As Integer<%ImplementsITokenStartPos%>
            Get
                Return m_startPos
            End Get
            Set(ByVal value As Integer)
                m_startPos = value
            End Set
        End Property

        Public Property EndPos() As Integer<%ImplementsITokenEndPos%>
            Get
                Return m_endPos
            End Get
            Set(ByVal value As Integer)
                m_endPos = value
            End Set
        End Property

        Public ReadOnly Property Length() As Integer<%ImplementsITokenLength%>
            Get
                Return m_endPos - m_startPos
            End Get
        End Property

        Public Property Text() As String<%ImplementsITokenText%>
            Get
                Return m_text
            End Get
            Set(ByVal value As String)
                m_text = value
            End Set
        End Property

        Public Property Skipped() As List(Of Token)
            Get
                Return m_skipped
            End Get
            Set(ByVal value As List(Of Token))
                m_skipped = value
            End Set
        End Property

        Public Property Value() As Object
            Get
                Return m_value
            End Get
            Set(ByVal value As Object)
                Me.m_value = value
            End Set
        End Property

        <XmlAttribute()>
        Public Type As TokenType

        Public Sub New()
            Me.New(0, 0)
        End Sub

        Public Sub New(ByVal start As Integer, ByVal endPos As Integer)
            Type = TokenType._UNDETERMINED_
            m_startPos = start
            m_endPos = endPos
            Text = ""
            ' must initialize with empty string, may cause null reference exceptions otherwise
            Value = Nothing
        End Sub

        Public Sub UpdateRange(ByVal token As Token)
            If token.StartPos < m_startPos Then
                m_startPos = token.StartPos
            End If
            If token.EndPos > m_endPos Then
                m_endPos = token.EndPos
            End If
        End Sub

        Public Overloads Overrides Function ToString() As String<%ImplementsITokenToString%>
            If Text <> Nothing Then
                Return Type.ToString() + " '" + Text + "'"
            Else
                Return Type.ToString()
            End If
        End Function

        <%ScannerCustomCode%>

End Class
#End Region
End Namespace
