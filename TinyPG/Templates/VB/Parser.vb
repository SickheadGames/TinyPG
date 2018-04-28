'Automatically generated from source file: <%SourceFilename%>
'By TinyPG v1.3 available at http://github.com/SickheadGames/TinyPG

Imports System
Imports System.Collections.Generic
<%Imports%>

Namespace <%Namespace%>
#Region "Parser"

    Partial Public Class Parser <%IParser%>
        Private m_scanner As Scanner
        Private m_tree As ParseTree

        Public Sub New(ByVal scanner As Scanner)
            m_scanner = scanner
        End Sub


        Public Function Parse(ByVal input As String) As <%IParseTree%>
            m_tree = New ParseTree()
            Return Parse(input, m_tree)
        End Function

        Public Function Parse(ByVal input As String, ByVal tree As ParseTree) As ParseTree
            m_scanner.Init(input)

            m_tree = tree
            ParseStart(m_tree)
            m_tree.Skipped = m_scanner.Skipped

            Return m_tree
        End Function

        <%ParseNonTerminals%>

<%ParserCustomCode%>
    End Class
#End Region
End Namespace

